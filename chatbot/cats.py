import requests
from collections import defaultdict
import json
import yaml
url = "https://places.demo.api.here.com/places/v1/categories/places"

querystring = {"app_id":"nCSzEMs5Mt4xNwpSu67q","app_code":"BKcZWZqhrhY2sMaIlmKh6Q"}

headers = {
    'User-Agent': "PostmanRuntime/7.15.2",
    'Accept': "*/*",
    'Cache-Control': "no-cache",
    'Postman-Token': "77e025f0-2d0e-4015-a163-68c57fe2fce6,9ff1ef59-fdc0-4d8a-b996-092b81bd5149",
    'Host': "places.demo.api.here.com",
    'Accept-Encoding': "gzip, deflate",
    'Accept-Language': 'ru-RU',
    'Connection': "keep-alive",
    'cache-control': "no-cache"
    }

response = requests.request("GET", url, headers=headers, params=querystring)

result = response.json()["items"]
print(type(result))

top_level = set()
# categories = defaultdict(lambda: {"id": None, "title": None, "subcategories": []})
categories = []

for pt in result:
    categories.append({
            "id": pt['id'],
            "title": pt['title']
    })

    # if len(pt['within'])>0:
        # categories[pt['within'][0]]["subcategories"].append({
            # "id": pt['id'],
            # "title": pt['title'],
        # })
    # else:
        # categories[pt['id']]['id']=pt['id']
        # categories[pt['id']]['title']=pt['title']
print(categories)
yaml.dump(list(categories), open("cats.json", 'w'), allow_unicode=True)





