using MeetingPointAPI.Entities;
using MeetingPointAPI.Helpers;
using MeetingPointAPI.Models;
using MeetingPointAPI.Repositories;
using MeetingPointAPI.Services.Interfaces;
using MeetingPointAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly IRouteSearcher _routeSearcher;
        private readonly AppSettings _appSettings;

        public MeetingPointController(AppSettings appSettings, IHereService hereService, DBRepository dbRepository, IRouteSearcher routeSearcher)
        {
            _hereService = hereService;
            _dbRepository = dbRepository;
            _routeSearcher = routeSearcher;
            _appSettings = appSettings;
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
            if (memberLocations == null)
                return NoContent();

            var targetPoint = CoordinateHelper.GetTargetCoordinate(memberLocations.Select(m => m.GetCoordinate()));

            var result = await GetPlaces(targetPoint, groupMemberLocationVM.Category);
            if (result == null)
                return NoContent();

            await SavePotentialRoutes(groupMemberLocationVM.GroupUid, memberLocations, groupMemberLocationVM.Time ?? DateTime.Now, result.Results.Items);

            var webSiteEndpoint = $"{_appSettings.WebSiteDomain}/result/{groupMemberLocationVM.GroupUid}";
            return Ok(webSiteEndpoint);
        }

        /// <summary> Метод получения результата по сбору группы </summary>
        [HttpGet(nameof(GetResult) + "/{groupUid}")]
        public async Task<IActionResult> GetResult([FromRoute]Guid groupUid)
        {
            var routes = await _dbRepository.GetPotentialMembersRoutes(groupUid);

            var result = routes.Select(route => new RoutesToPlace
            {
                GroupUid = route.MemberRoutes.GroupUid,
                Place = ModelConverter.ToPlace(route.Place),
                MemberRoutes = JsonConvert.DeserializeObject<List<MemberRoute>>(route.MemberRoutes.MemberRoutes)
            });

            return Ok(result);
        }

        private async Task SavePotentialRoutes(Guid groupUid, List<MemberLocationEntity> memberLocations, DateTime time, IEnumerable<HereExploreItem> places)
        {
            await _dbRepository.RemoveAllRoutes(groupUid);

            var groupRoutes = (await Task.WhenAll(places.Take(_appSettings.PlacesLimit).Select(place =>
                _routeSearcher.GetGroupRoutes(memberLocations, place.Title, place.GetCoordinate(), time)))).ToList();

            var locations = await _dbRepository.InsertLocations(places.Take(_appSettings.PlacesLimit).Select(place => new LocationEntity
            {
                Title = place.Title,
                Longitude = place.Position[1],
                Latitude = place.Position[0],
                Category = place.Category?.Title,
                Distance = place.Distance,
                Href = place.Href,
                Icon = place.Icon,
                Vicinity = place.Vicinity,
                Type = place.Type
            }));

            await _dbRepository.InsertRoutes(groupRoutes.Select(groupRoute => new RouteEntity
            {
                GroupUid = groupUid,
                LocationId = locations.First(location => location.Title == groupRoute.Title).Id,
                SumTime = (int)groupRoute.SumTime,
                MemberRoutes = JsonConvert.SerializeObject(groupRoute.MemberRoutes)
            }));
        }

        private async Task<HereExploreResponseResult> GetPlaces(Coordinate targetPoint, IEnumerable<string> categories)
        {
            var radiuses = new List<int> { 500, 1000, 5000, 10000, 50000, 100000 };
            foreach(var radius in radiuses)
            {
                var result = await _hereService.GetPlaces(targetPoint, categories, radius);
                if (result != null && result?.Results?.Items != null && result.Results.Items.Any())
                    return result;
            }
            return null;
        }
    }
}
