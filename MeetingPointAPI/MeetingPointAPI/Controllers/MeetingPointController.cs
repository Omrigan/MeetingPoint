using MeetingPointAPI.Services.Interfaces;
using MeetingPointAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeetingPointAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("AllowAll")]
    public class MeetingPointController : Controller
    {
        private readonly IHereService _hereService;

        public MeetingPointController(IHereService hereService)
        {
            _hereService = hereService;
        }

        /// <summary> Метод добавления местоположения участника группы </summary>
        [HttpPost(nameof(AddLocation))]
        public async Task<IActionResult> AddLocation([FromBody]GroupMemberLocationVM groupMemberLocationVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }
    }
}
