using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadRoleRestrictionRepository : IRaidEncounterSquadRoleRestrictionRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadRoleRestrictionRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquadRoleRestriction> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestriction>("SELECT id, raid_encounter_squad_role_id, profession_id, minimum_guild_rank_id FROM raid_encounter_squad_role_restriction");
            }
        }

        public RaidEncounterSquadRoleRestriction Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestriction>("SELECT id, raid_encounter_squad_role_id, profession_id, minimum_guild_rank_id FROM raid_encounter_squad_role_restriction WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public IEnumerable<RaidEncounterSquadRoleRestriction> LoadRoleRestrictions(int squadRoleId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestriction>("SELECT id, raid_encounter_squad_role_id, profession_id, minimum_guild_rank_id FROM raid_encounter_squad_role_restriction WHERE raid_encounter_squad_role_id = @RaidEncounterSquadRoleId", new {RaidEncounterSquadRoleId = squadRoleId});
            }
        }

        public RaidEncounterSquadRoleRestriction Save(RaidEncounterSquadRoleRestriction raidEncounterSquadRoleRestriction)
        {
            if (raidEncounterSquadRoleRestriction.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad_role_restriction 
                    SET 
                        raid_encounter_squad_role_id = @RaidEncounterSquadRoleId,
                        profession_id = @ProfessionId,
                        minimum_guild_rank_id = @MinimumGuildRankId
                    WHERE 
                        id = @Id
                    ", raidEncounterSquadRoleRestriction);

                    return raidEncounterSquadRoleRestriction;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_squad_role_restriction (raid_encounter_squad_role_id, profession_id, minimum_guild_rank_id) 
                    VALUES (@RaidEncounterSquadRoleId, @ProfessionId, @MinimumGuildRankId)           
                    RETURNING id         
                    ", raidEncounterSquadRoleRestriction).Single();

                    raidEncounterSquadRoleRestriction.Id = id;
                    return raidEncounterSquadRoleRestriction;
                }
            }
        }

        public RaidEncounterSquadRoleRestriction Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounterSquadRoleRestriction = Load(id);
                if (raidEncounterSquadRoleRestriction != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_role_restriction WHERE id = @id", new {id = id});
                    raidEncounterSquadRoleRestriction.Id = 0;
                }

                return raidEncounterSquadRoleRestriction;
            }
        }
    }
}