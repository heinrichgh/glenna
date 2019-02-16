using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidEncounterSquadTemplateRepository : IRaidEncounterSquadTemplateRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidEncounterSquadTemplateRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidEncounterSquadTemplate> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadTemplate>("SELECT id, raid_encounter_template_id, position FROM raid_encounter_squad_template");
            }
        }

        public IEnumerable<RaidEncounterSquadTemplate> LoadSquad(int raidEncounterTemplateId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadTemplate>("SELECT id, raid_encounter_template_id, position FROM raid_encounter_squad_template WHERE raid_encounter_template_id = @RaidEncounterTemplateId", new {RaidEncounterTemplateId = raidEncounterTemplateId});
            }
        }

        public RaidEncounterSquadTemplate Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidEncounterSquadTemplate>("SELECT id, raid_encounter_template_id, position FROM raid_encounter_squad_template WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidEncounterSquadTemplate Save(RaidEncounterSquadTemplate raidEncounterSquadTemplate)
        {
            if (raidEncounterSquadTemplate.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_encounter_squad_template 
                    SET 
                        raid_encounter_template_id = @RaidEncounterTemplateId,
                        position = @Position,
                    WHERE 
                        id = @Id
                    ", raidEncounterSquadTemplate);

                    return raidEncounterSquadTemplate;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_encounter_squad_template (raid_encounter_template_id, position) 
                    VALUES (@RaidEncounterTemplateId, @Position)           
                    RETURNING id         
                    ", raidEncounterSquadTemplate).Single();

                    raidEncounterSquadTemplate.Id = id;
                    return raidEncounterSquadTemplate;
                }
            }
        }

        public RaidEncounterSquadTemplate Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounterSquad = Load(id);
                if (raidEncounterSquad != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_template WHERE id = @id", new {id = id});
                    raidEncounterSquad.Id = 0;
                }

                return raidEncounterSquad;
            }
        }
        public IEnumerable<RaidEncounterSquadTemplate> RemoveSquad(int raidEncounterSquadTemplateId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidEncounter = LoadSquad(raidEncounterSquadTemplateId);
                if (raidEncounter != null)
                {
                    dbConnection.Execute("DELETE FROM raid_encounter_squad_template WHERE raid_encounter_template_id = @RaidEncounterSquadTemplateId", new {RaidEncounterSquadTemplateId = raidEncounterSquadTemplateId});
                }

                return raidEncounter;
            }
        }
    }
}