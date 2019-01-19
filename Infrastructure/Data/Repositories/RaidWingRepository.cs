using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidWingRepository : IRepository<RaidWing>
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidWingRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidWing> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidWing>("SELECT id, name FROM raid_wing");
            }
        }

        public RaidWing Load(uint id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidWing>("SELECT id, name FROM raid_wing WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }
    }
}