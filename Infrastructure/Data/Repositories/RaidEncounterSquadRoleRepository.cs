using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadRoleRepository : IRaidEncounterSquadRoleRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadRoleRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquadRole> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRole>("SELECT id, raid_encounter_squad_id, raid_role_id FROM raid_encounter_squad_role");
            }
        }

        public IEnumerable<RaidEncounterSquadRole> LoadSquadRole(int raidEncounterSquadId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRole>("SELECT id, raid_encounter_squad_id, raid_role_id FROM raid_encounter_squad_role WHERE raid_encounter_squad_id = @RaidEncounterSquadId", new {RaidEncounterSquadId = raidEncounterSquadId});
            }
        }

        public RaidEncounterSquadRole Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRole>("SELECT id, raid_encounter_squad_id, raid_role_id FROM raid_encounter_squad_role WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidEncounterSquadRole Save(RaidEncounterSquadRole raidEncounterSquadRole)
        {
            if (raidEncounterSquadRole.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad_role 
                    SET 
                        raid_encounter_squad_id = @RaidEncounterSquadId,
                        raid_role_id = @RaidRoleId,
                    WHERE 
                        id = @Id
                    ", raidEncounterSquadRole);

                    return raidEncounterSquadRole;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_squad_role (raid_encounter_squad_id, raid_role_id) 
                    VALUES (@RaidEncounterSquadId, @RaidRoleId)           
                    RETURNING id         
                    ", raidEncounterSquadRole).Single();

                    raidEncounterSquadRole.Id = id;
                    return raidEncounterSquadRole;
                }
            }
        }

        public RaidEncounterSquadRole Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = Load(id);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_role WHERE id = @id", new {id = id});
                    raidEncounter.Id = 0;
                }

                return raidEncounter;
            }
        }
        public IEnumerable<RaidEncounterSquadRole> RemoveSquadRole(int raidEncounterSquadId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounterSquadRole = LoadSquadRole(raidEncounterSquadId);
                if (raidEncounterSquadRole != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_role WHERE raid_encounter_squad_id = @RaidEncounterSquadId", new {RaidEncounterSquadId = raidEncounterSquadId});
                }

                return raidEncounterSquadRole;
            }
        }
    }
}