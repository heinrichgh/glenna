using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterRepository : IRaidEncounterRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounter> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounter>("SELECT id, raid_id, raid_boss_id FROM raid_encounter");
            }
        }

        public RaidEncounter Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounter>("SELECT id, raid_id, raid_boss_id FROM raid_encounter WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidEncounter Load(int raidId, int raidBossId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounter>("SELECT id, raid_id, raid_boss_id FROM raid_encounter WHERE raid_id = @RaidId AND raid_boss_id = @RaidBossId", new {RaidId = raidId, RaidBossId = raidBossId}).FirstOrDefault();
            }
        }
        
        public RaidEncounter Save(RaidEncounter raidEncounter)
        {
            if (raidEncounter.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter 
                    SET 
                        raid_id = @RaidId,
                        raid_boss_id = @RaidBossId
                    WHERE 
                        id = @Id
                    ", raidEncounter);

                    return raidEncounter;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter (raid_id, raid_boss_id) 
                    VALUES (@RaidId, @RaidBossId)           
                    RETURNING id         
                    ", raidEncounter).Single();

                    raidEncounter.Id = id;
                    return raidEncounter;
                }
            }
        }

        public RaidEncounter Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = Load(id);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter WHERE id = @id", new {id = id});
                    raidEncounter.Id = 0;
                }

                return raidEncounter;
            }
        }
    }
}