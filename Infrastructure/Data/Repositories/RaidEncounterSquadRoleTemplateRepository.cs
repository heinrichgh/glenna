using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadRoleTemplateRepository : IRaidEncounterSquadRoleTemplateRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadRoleTemplateRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquadRoleTemplate> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleTemplate>("SELECT id, raid_encounter_squad_template_id, raid_role_id FROM raid_encounter_squad_role_template");
            }
        }

        public RaidEncounterSquadRoleTemplate LoadSquadRole(int raidEncounterTemplateId, int position)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleTemplate>(@"
                SELECT raid_encounter_squad_role_template.id, raid_encounter_squad_template_id, raid_role_id 
                FROM raid_encounter_squad_role_template 
                JOIN raid_encounter_squad_template ON raid_encounter_squad_role_template.raid_encounter_squad_template_id = raid_encounter_squad_template.id
                WHERE raid_encounter_squad_template.raid_encounter_template_id = @RaidEncounterTemplateId
				AND raid_encounter_squad_template.position = @Position", new {RaidEncounterTemplateId = raidEncounterTemplateId, Position = position}).FirstOrDefault();
            }
        }

        public RaidEncounterSquadRoleTemplate Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadRoleTemplate>("SELECT id, raid_encounter_squad_template_id, raid_role_id FROM raid_encounter_squad_role_template WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidEncounterSquadRoleTemplate Save(RaidEncounterSquadRoleTemplate raidEncounterSquadRoleTemplate)
        {
            if (raidEncounterSquadRoleTemplate.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad_role_template 
                    SET 
                        raid_encounter_squad_template_id = @RaidEncounterSquadTemplateId,
                        raid_role_id = @RaidRoleId,
                    WHERE 
                        id = @Id
                    ", raidEncounterSquadRoleTemplate);

                    return raidEncounterSquadRoleTemplate;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_squad_role_template (raid_encounter_squad_template_id, raid_role_id) 
                    VALUES (@RaidEncounterSquadTemplateId, @RaidRoleId)           
                    RETURNING id         
                    ", raidEncounterSquadRoleTemplate).Single();

                    raidEncounterSquadRoleTemplate.Id = id;
                    return raidEncounterSquadRoleTemplate;
                }
            }
        }

        public RaidEncounterSquadRoleTemplate Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = Load(id);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_role_template WHERE id = @id", new {id = id});
                    raidEncounter.Id = 0;
                }

                return raidEncounter;
            }
        }
        public IEnumerable<RaidEncounterSquadRoleTemplate> RemoveSquadRole(int raidEncounterSquadTemplateId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                dbConnection.Execute("DELETE FROM raid_encounter_squad_role_template WHERE raid_encounter_squad_template_id = @RaidEncounterSquadTemplateId", new {RaidEncounterSquadTemplateId = raidEncounterSquadTemplateId});
            

                return null;
            }
        }
    }
}