using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadRoleRestrictionTemplateRepository : IRaidEncounterSquadRoleRestrictionTemplateRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadRoleRestrictionTemplateRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquadRoleRestrictionTemplate> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestrictionTemplate>("SELECT id, raid_encounter_squad_role_template_id, profession_id, minimum_guild_rank_id FROM raid_encounter_squad_role_restriction_template");
            }
        }


        public RaidEncounterSquadRoleRestrictionTemplate Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestrictionTemplate>("SELECT id, raid_encounter_squad_role_template_id, profession_id, minimum_guild_rank_id FROM raid_encounter_squad_role_restriction_template WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public IEnumerable<RaidEncounterSquadRoleRestrictionTemplate> LoadByRole(int roleTempalteId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleRestrictionTemplate>(@"
                SELECT id, raid_encounter_squad_role_template_id, profession_id, minimum_guild_rank_id 
                FROM raid_encounter_squad_role_restriction_template 
                WHERE raid_encounter_squad_role_template_id = @RaidEncounterSquadRoleTemplateId", new {RaidEncounterSquadRoleTemplateId = roleTempalteId});
            }
        }

        public RaidEncounterSquadRoleRestrictionTemplate Save(RaidEncounterSquadRoleRestrictionTemplate raidEncounterSquadRoleRestrictionTemplate)
        {
            if (raidEncounterSquadRoleRestrictionTemplate.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad_role_restriction_template 
                    SET 
                        raid_encounter_squad_role_template_id = @RaidEncounterSquadRoleTemplateId,
                        profession_id = @ProfessionId,
                        minimum_guild_rank_id = @MinimumGuildRankId
                    WHERE 
                        id = @Id
                    ", raidEncounterSquadRoleRestrictionTemplate);

                    return raidEncounterSquadRoleRestrictionTemplate;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_squad_role_restriction_template (raid_encounter_squad_role_template_id, profession_id, minimum_guild_rank_id) 
                    VALUES (@RaidEncounterSquadRoleTemplateId, @ProfessionId, @MinimumGuildRankId)           
                    RETURNING id         
                    ", raidEncounterSquadRoleRestrictionTemplate).Single();

                    raidEncounterSquadRoleRestrictionTemplate.Id = id;
                    return raidEncounterSquadRoleRestrictionTemplate;
                }
            }
        }

        public RaidEncounterSquadRoleRestrictionTemplate Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounterSquadRoleRestrictionTemplate = Load(id);
                if (raidEncounterSquadRoleRestrictionTemplate != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_role_restriction_template WHERE id = @id", new {id = id});
                    raidEncounterSquadRoleRestrictionTemplate.Id = 0;
                }

                return raidEncounterSquadRoleRestrictionTemplate;
            }
        }
    }
}