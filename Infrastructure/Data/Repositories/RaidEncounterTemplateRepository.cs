using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterTemplateRepository : IRaidEncounterTemplateRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterTemplateRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterTemplate> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterTemplate>("SELECT id, raid_template_id, raid_boss_id FROM raid_encounter_template");
            }
        }

        public RaidEncounterTemplate Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterTemplate>("SELECT id, raid_template_id, raid_boss_id FROM raid_encounter_template WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public IEnumerable<RaidEncounterTemplate> LoadByTemplate(int raidTemplateId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterTemplate>("SELECT id, raid_template_id, raid_boss_id FROM raid_encounter_template WHERE raid_template_id = @RaidTemplateId ", new {RaidTemplateId = raidTemplateId});
            }
        }

        public RaidEncounterTemplate Load(int raidTemplateId, int raidBossId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterTemplate>("SELECT id, raid_template_id, raid_boss_id FROM raid_encounter_template WHERE raid_template_id = @RaidTemplateId AND raid_boss_id = @RaidBossId", new {RaidTemplateId = raidTemplateId, RaidBossId = raidBossId}).FirstOrDefault();
            }
        }
        
        public RaidEncounterTemplate Save(RaidEncounterTemplate raidEncounterTemplate)
        {
            if (raidEncounterTemplate.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_template
                    SET 
                        raid_template_id = @RaidTemplateId,
                        raid_boss_id = @RaidBossId
                    WHERE 
                        id = @Id
                    ", raidEncounterTemplate);

                    return raidEncounterTemplate;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_template (raid_template_id, raid_boss_id) 
                    VALUES (@RaidTemplateId, @RaidBossId)           
                    RETURNING id         
                    ", raidEncounterTemplate).Single();

                    raidEncounterTemplate.Id = id;
                    return raidEncounterTemplate;
                }
            }
        }

        public RaidEncounterTemplate Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = Load(id);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_template WHERE id = @id", new {id = id});
                    raidEncounter.Id = 0;
                }

                return raidEncounter;
            }
        }
    }
}