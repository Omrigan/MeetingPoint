using Dapper;
using MeetingPointAPI.Entities;
using MeetingPointAPI.Models;
using MeetingPointAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPointAPI.Repositories
{
    public class DBRepository
    {
        private readonly string _connectionString;

        public DBRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task InsertMemberLocation(GroupMemberLocationVM groupMemberLocation)
        {
            var sqlCommand = new CommandDefinition(@"
                insert [dbo].[MemberLocations] (MemberId, GroupUid, Latitude, Longitude)
                values (@memberId, @groupUid, @latitude, @longitude)",
                new
                {
                    @memberId = groupMemberLocation.MemberId,
                    @groupUid = groupMemberLocation.GroupUid,
                    @latitude = groupMemberLocation.Coordinate.Latitude,
                    @longitude = groupMemberLocation.Coordinate.Longitude
                });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task UpdateLocation(GroupMemberLocationVM groupMemberLocation)
        {
            var sqlCommand = new CommandDefinition(@"
                update [dbo].[MemberLocations]
                set 
                    Latitude = @latitude, 
                    Longitude = @longitude
                where MemberId = @memberId and GroupUid = @groupUid",
                new
                {
                    @memberId = groupMemberLocation.MemberId,
                    @groupUid = groupMemberLocation.GroupUid,
                    @latitude = groupMemberLocation.Coordinate.Latitude,
                    @longitude = groupMemberLocation.Coordinate.Longitude
                });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task UpdateLocation(int locatioId, Coordinate coordinate)
        {
            var sqlCommand = new CommandDefinition(@"
                update [dbo].[MemberLocations]
                set 
                    Latitude = @latitude, 
                    Longitude = @longitude
                where Id = @locationId",
                new
                {
                    @locationId = locatioId,
                    @latitude = coordinate.Latitude,
                    @longitude = coordinate.Longitude
                });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task InsertOrUpdateLocation(GroupMemberLocationVM groupMemberLocation)
        {
            var locationId = await GetMemberLocationId(groupMemberLocation.MemberId, groupMemberLocation.GroupUid);
            if (!locationId.HasValue)
                await InsertMemberLocation(groupMemberLocation);
            else
                await UpdateLocation(locationId.Value, groupMemberLocation.Coordinate);
        }

        public async Task<int?> GetMemberLocationId(string memberId, Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                select top 1 Id
                from [dbo].[MemberLocations]
                where MemberId = @memberId and GroupUid = @groupUid",
                new
                {
                    @memberId = memberId,
                    @groupUid = groupUid
                });

            using (var conn = CreateConnection())
                return await conn.QueryFirstOrDefaultAsync(typeof(int), sqlCommand) as int?;
        }

        public async Task DeleteLocation(string memberId, Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                delete [dbo].[MemberLocations]
                where MemberId = @memberId and GroupUid = @groupUid",
                new
                {
                    @memberId = memberId,
                    @groupUid = groupUid
                });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task DeleteGroupLocations(Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                delete [dbo].[MemberLocations]
                where GroupUid = @groupUid",
                new { @groupUid = groupUid });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task<IEnumerable<MemberLocationEntity>> GetGroupMemberLocations(Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                select *
                from [dbo].[MemberLocations]
                where GroupUid = @groupUid",
                new { @groupUid = groupUid });

            using (var conn = CreateConnection())
                return await conn.QueryAsync<MemberLocationEntity>(sqlCommand);
        }

        public async Task RemoveAllRoutes(Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                delete [dbo].[Routes]
                where GroupUid = @groupUid",
                new { @groupUid = groupUid });

            using (var conn = CreateConnection())
                await conn.ExecuteAsync(sqlCommand);
        }

        public async Task<IEnumerable<RouteEntity>> InsertRoutes(IEnumerable<RouteEntity> routes)
        {
            if (routes == null || !routes.Any())
                return Enumerable.Empty<RouteEntity>();

            return await Task.WhenAll(routes.Select(route => InsertRoute(route)));
        }

        public async Task<RouteEntity> InsertRoute(RouteEntity routeEntity)
        {
            var sqlCommand = new CommandDefinition(@"
                insert [dbo].[Routes] (GroupUid, LocationId, MemberRoutes, SumTime)
                values (@groupUid, @locationId, @memberRoutes, @sumTime);

                SELECT CAST(SCOPE_IDENTITY() as int)",
            new
            {
                @groupUid = routeEntity.GroupUid,
                @locationId = routeEntity.LocationId,
                @memberRoutes = routeEntity.MemberRoutes,
                @sumTime = routeEntity.SumTime
            });

            using (var conn = CreateConnection())
            {
                routeEntity.Id = (await conn.QueryFirstOrDefaultAsync(typeof(int), sqlCommand) as int?).Value;
                return routeEntity;
            }
        }

        public async Task<IEnumerable<RouteEntity>> GetRoutes(Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                select *
                from [dbo].[Routes]
                where GroupUid = @groupUid",
            new { @groupUid = groupUid });

            using (var conn = CreateConnection())
                return await conn.QueryAsync<RouteEntity>(sqlCommand);
        }

        public async Task<IEnumerable<LocationEntity>> InsertLocations(IEnumerable<LocationEntity> locations)
        {
            if (locations == null || !locations.Any())
                return Enumerable.Empty<LocationEntity>();

            return await Task.WhenAll(locations.Select(location => InsertLocation(location)));
        }

        public async Task<LocationEntity> InsertLocation(LocationEntity location)
        {
            var sqlCommand = new CommandDefinition(@"
                insert [dbo].[Locations] (Title, Longitude, Latitude, Type, Vicinity, Icon, Href, Distance, Category)
                values (@title, @longitude, @latitude, @type, @vicinity, @icon, @href, @distance, @category);

                SELECT CAST(SCOPE_IDENTITY() as int)",
                new
                {
                    @title = location.Title,
                    @longitude = location.Longitude,
                    @latitude = location.Latitude,
                    @type = location.Type,
                    @vicinity = location.Vicinity,
                    @icon = location.Icon,
                    @href = location.Href,
                    @distance = location.Distance,
                    @category = location.Category
                });

            using (var conn = CreateConnection())
            {
                location.Id = (await conn.QueryFirstOrDefaultAsync(typeof(int), sqlCommand) as int?).Value;
                return location;
            }
        }

        public async Task<IEnumerable<RoutesToLocationEntity>> GetPotentialMembersRoutes(Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                SELECT Routes.*, Locations.*
                FROM [dbo].[Routes]
                join [dbo].[Locations] on Locations.Id = Routes.LocationId
                where Routes.GroupUid = @groupUid
                order by SumTime",
                new { @groupUid = groupUid });

            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<RouteEntity, LocationEntity, RoutesToLocationEntity>(sqlCommand, 
                    (memberRoute, location) => new RoutesToLocationEntity
                    {
                        MemberRoutes = memberRoute,
                        Place = location
                    }, "Id");
            }
        }
    }
}
