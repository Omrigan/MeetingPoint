import logging
import telegram
import tinydb
from telegram.error import NetworkError, Unauthorized
from time import sleep
from tinydb import TinyDB, Query
import requests
import yaml



db = TinyDB('db.json')

update_id = None
meetings = db.table('meetings')


def main():
    """Run the bot."""
    global update_id
    # Telegram Bot Authorization Token
    bot = telegram.Bot('901283198:AAGjc7sRoGmMOgw17u1Z-Iw-k4SzEmT952c')

    # get the first pending update_id, this is so we can skip over it in case
    # we get an "Unauthorized" exception.
    try:
        update_id = bot.get_updates()[0].update_id
    except IndexError:
        update_id = None

    logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')

    while True:
        try:
            echo(bot)
        except NetworkError:
            sleep(1)
        except Unauthorized:
            # The user has removed or blocked the bot.
            update_id += 1


def echo(bot):
    """Echo the message the user sent."""
    global update_id
    # Request updates after the last update_id
    for update in bot.get_updates(offset=update_id, timeout=10):
        update_id = update.update_id + 1
        process(update)

def get_meeting(chat):
    q = Query().chats.any([chat.id])
    results = meetings.search(q)
    if len(results)==0:
        return None
    return results[0]

def save_meeting(meeting):
    meetings.update(meeting, Query().guid == meeting['guid'])


cats = yaml.load(open('cats-filtered.yaml'))
title_to_id = {elem['title']: elem['id'] for elem in cats}
id_to_title = {elem['id']: elem['title'] for elem in cats}

def get_cats_keyboard(meeting):
    custom_keyboard = [["Готово"]]
    for elem in cats:
        if elem['id'] in meeting['selected_categories']:
            custom_keyboard.append(["Удалить %s" % elem['title']])
        else:
            custom_keyboard.append(["Добавить %s" % elem['title']])
    reply_markup = telegram.ReplyKeyboardMarkup(custom_keyboard)
    return reply_markup


def new_meeting(update, chat):
    meeting = {"guid": chat['id'],
                    "chats": [chat['id']],
                    "locations": [],
                    'state': 'categories',
                    'selected_categories': ['coffee', 'tea']
                    }
    meetings.insert(meeting)
    update.message.reply_text("Начинаем встречу в этом чате. Какие категории вам интересны?", reply_markup=get_cats_keyboard(meeting))

def update_categories(update, meeting):
    txt = update.message.text
    if not txt:
        return False
    fields = txt.split(" ")
    cat_title = " ".join(fields[1:])
    cat_id = title_to_id.get(cat_title)

    result_text = None
    result_markup = None
    if txt.startswith("Добавить") :
        meeting['selected_categories'].append(cat_id)
        result_text = "Добавляем %s." % cat_title 
        result_markup = get_cats_keyboard(meeting)
    elif txt.startswith("Удалить") :
        if cat_id in meeting['selected_categories']:
            meeting['selected_categories'].remove(cat_id)
        result_text = "Удаляем %s." % cat_title 
        result_markup = get_cats_keyboard(meeting)
    elif txt.startswith("Готово"):
        result_markup = telegram.ReplyKeyboardRemove() 
        meeting['state'] = 'await'
        result_text = "Начинаем сбор локаций."
    else:
        return False 
    result_text+= " Категории: " + ", ".join(id_to_title[id] for id in meeting['selected_categories'])
    print(meeting['selected_categories'])
    update.message.reply_text(result_text, reply_markup=result_markup)
    save_meeting(meeting)



def write_location(update, meeting, location):
    uid = update.effective_user.id 
    done = False
    uid = "@" + update.effective_user.username
    location = {'longitude': location.longitude, 'latitude': location.latitude} 
    for item in meeting["locations"]:
        if item['uid']==uid:
            item["location"] = location
            done = True
            update.message.reply_text("Локация от %s обновлена" % uid)

    if not done:
        meeting["locations"].append({"uid": uid, "location": location})
        update.message.reply_text("Локация от %s записана" % uid)
    save_meeting(meeting)



def calculate(update, meeting):
    link = "test"
    update.message.reply_text("Ссылка на результат: %s" % link)

def clear(update, meeting):
    meetings.remove(Query().guid==meeting['guid'])
    update.message.reply_text("Завершаем встречу")
    
    

# def generate_invte(guid):

# def resolve_invite(invite):





def process(update):
    if update.message:  # your bot can receive updates without messages
        meeting = get_meeting(update.message.chat)
        if update.message.text=="/meet":
            if meeting:
                update.message.reply_text("Встреча уже начата")
                return
            new_meeting(update, update.message.chat)
            return
        if not meeting:
            update.message.reply_text("Сбор встречи еще не начат. Используйте /meet")
            return
        if update.message.location:
            location = update.message.location
            write_location(update, meeting, location)
            return
        if meeting['state']=='categories':
            update_categories(update, meeting)
            return
        if update.message.text=="/calculate":
            calculate(update, meeting)
            return
        if update.message.text=="/done":
            clear(update, meeting)
            return
        update.message.reply_text("Ничего не понял :(", reply_markup=None)



if __name__ == '__main__':
    print("Starting")
    main()
