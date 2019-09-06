using Dapper;
using MeetingPointAPI.Models;
using MeetingPointAPI.ViewModels;
using System;
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

        public async Task InsertLocation(GroupMemberLocationVM groupMemberLocation)
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
                await InsertLocation(groupMemberLocation);
            else
                await UpdateLocation(locationId.Value, groupMemberLocation.Coordinate);
        }

        public async Task<int?> GetMemberLocationId(string memberId, Guid groupUid)
        {
            var sqlCommand = new CommandDefinition(@"
                select Id
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
    }
}
