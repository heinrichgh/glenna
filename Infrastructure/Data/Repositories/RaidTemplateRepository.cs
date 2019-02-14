using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidTemplateRepository : IRaidTemplateRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidTemplateRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidTemplate> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidTemplate>("SELECT id, guild_id, name FROM raid_template");
            }
        }

        public RaidTemplate Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidTemplate>("SELECT id, guild_id, name FROM raid_template WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidTemplate Save(RaidTemplate raidTemplate)
        {
            if (raidTemplate.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_template 
                    SET 
                        guild_id = @GuildId,
                        name = @Name
                    WHERE 
                        id = @Id
                    ", raidTemplate).Single();

                    return raidTemplate;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_template (guild_id, name) 
                    VALUES (@GuildId, @Name)           
                    RETURNING id         
                    ", raidTemplate).Single();

                    raidTemplate.Id = id;
                    return raidTemplate;
                }
            }
        }

        public RaidTemplate Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidTemplate = Load(id);
                if (raidTemplate != null)
                {
                    dbConnection.Execute("DELETE FROM raid_template WHERE id = @id", new {id = id});
                    raidTemplate.Id = 0;
                }

                return raidTemplate;
            }
        }
    }
}