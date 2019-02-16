using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadRepository : IRaidEncounterSquadRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquad> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquad>("SELECT id, raid_encounter_id, position, guild_member_id FROM raid_encounter_squad");
            }
        }

        public IEnumerable<RaidEncounterSquad> LoadSquad(int raidEncounterId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquad>("SELECT id, raid_encounter_id, position, guild_member_id FROM raid_encounter_squad WHERE raid_encounter_id = @RaidEncounterId", new {RaidEncounterId = raidEncounterId});
            }
        }

        public RaidEncounterSquad Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquad>("SELECT id, raid_encounter_id, position, guild_member_id FROM raid_encounter_squad WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidEncounterSquad Save(RaidEncounterSquad raidEncounter)
        {
            if (raidEncounter.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad 
                    SET 
                        raid_encounter_id = @RaidEncounterId,
                        position = @Position,
                        guild_member_id = @GuildMemberId
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
                    INSERT INTO raid_encounter_squad (raid_encounter_id, position, guild_member_id) 
                    VALUES (@RaidEncounterId, @Position, NULL)           
                    RETURNING id         
                    ", raidEncounter).Single();

                    raidEncounter.Id = id;
                    return raidEncounter;
                }
            }
        }

        public RaidEncounterSquad Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = Load(id);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad WHERE id = @id", new {id = id});
                    raidEncounter.Id = 0;
                }

                return raidEncounter;
            }
        }
        public IEnumerable<RaidEncounterSquad> RemoveSquad(int raidEncounterId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = LoadSquad(raidEncounterId);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad WHERE raid_encounter_id = @RaidEncounterId", new {RaidEncounterId = raidEncounterId});
                }

                return raidEncounter;
            }
        }
    }
}