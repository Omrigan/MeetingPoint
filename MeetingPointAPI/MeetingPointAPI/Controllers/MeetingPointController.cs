using MeetingPointAPI.Helpers;
using MeetingPointAPI.Repositories;
using MeetingPointAPI.Services.Interfaces;
using MeetingPointAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPointAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("AllowAll")]
    public class MeetingPointController : Controller
    {
        private readonly IHereService _hereService;
        private readonly DBRepository _dbRepository;

        public MeetingPointController(IHereService hereService, DBRepository dbRepository)
        {
            _hereService = hereService;
            _dbRepository = dbRepository;
        }

        /// <summary> Метод добавления местоположения участника группы </summary>
        [HttpPost(nameof(AddLocation))]
        public async Task<IActionResult> AddLocation([FromBody]GroupMemberLocationVM groupMemberLocationVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _dbRepository.InsertOrUpdateLocation(groupMemberLocationVM);

            return Ok();
        }

        /// <summary> Метод удаления местоположения участника группы </summary>
        [HttpPost(nameof(RemoveLocation))]
        public async Task<IActionResult> RemoveLocation([FromBody]GroupMemberVM groupMemberLocationVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _dbRepository.DeleteLocation(groupMemberLocationVM.MemberId, groupMemberLocationVM.GroupUid);

            return Ok();
        }

        /// <summary> Метод удаления местоположения всех участников группы </summary>
        [HttpPost(nameof(RemoveAllLocations))]
        public async Task<IActionResult> RemoveAllLocations([FromBody]GroupVM groupMemberLocationVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _dbRepository.DeleteGroupLocations(groupMemberLocationVM.GroupUid);

            return Ok();
        }


        /// <summary> Метод расчета потенциальных точек сбора группы </summary>
        [HttpPost(nameof(Calculate))]
        public async Task<IActionResult> Calculate([FromBody]TargetGroupPointVM groupMemberLocationVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var memberLocations = (await _dbRepository.GetGroupMemberLocations(groupMemberLocationVM.GroupUid))?.ToList();
            if (memberLocations == null || memberLocations.Count <= 1)
                return Forbid();

            var locations = memberLocations.Select(m => m.GetCoordinate()).ToList();
            var targetPoint = CoordinateHelper.GetTargetCoordinate(locations);
            var radius = CoordinateHelper.GetMaxDistance(targetPoint, locations);

            var result = await _hereService.GetPlaces(targetPoint, groupMemberLocationVM.Category, (int)radius);

            return Ok(result);
        }

        /// <summary> Метод получения результата по сбору группы </summary>
        [HttpGet(nameof(GetResult) + "/{groupUid}")]
        public async Task<IActionResult> GetResult([FromRoute]Guid groupUid)
        {

            return Ok();
        }
    }
}
