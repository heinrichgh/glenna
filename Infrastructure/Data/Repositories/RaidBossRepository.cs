using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidBossRepository : IRaidBossRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidBossRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidBoss> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidBoss>("SELECT id, name, raid_boss.order, raid_wing_id, has_cm, short_name, emoji FROM raid_boss");
            }
        }

        public RaidBoss Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidBoss>("SELECT id, name, raid_boss.order, raid_wing_id, has_cm, short_name, emoji FROM raid_boss WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

       
        public RaidBoss Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidBoss = Load(id);
                if (raidBoss != null)
                {
                    dbConnection.Execute("DELETE FROM raid_boss WHERE id = @id", new {id = id});
                    raidBoss.Id = 0;
                }

                return raidBoss;
            }
        }
    }
}