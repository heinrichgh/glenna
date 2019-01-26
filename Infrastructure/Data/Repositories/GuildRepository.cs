using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class GuildRepository : IGuildRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public GuildRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<Guild> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Guild>("SELECT id, name, tag, guild_guid, guild_leader, created_at FROM guild");
            }
        }
        public Guild Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Guild>("SELECT id, name, tag, guild_guid, guild_leader, created_at FROM guild WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public Guild Load(Guid gameGuildGuid)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Guild>("SELECT id, name, tag, guild_guid, guild_leader, created_at FROM guild WHERE guild_guid = @GameGuid", new {GameGuid = gameGuildGuid}).FirstOrDefault();
            }
        }
        
        public Guild Save(Guild guild)
        {
            if (guild.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    guild.CreatedAt = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE guild 
                    SET 
                        name = @Name,
                        tag = @Tag,
                        guild_guid = @GuildGuid,
                        guild_leader = @GuildLeader,
                    WHERE 
                        id = @Id
                    ", guild).Single();

                    return guild;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    guild.CreatedAt = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO guild (name, tag, guild_guid, guild_leader, created_at) 
                    VALUES (@Name, @Tag, @GuildGuid, @GuildLeader, @CreatedAt)           
                    RETURNING id         
                    ", guild).Single();

                    guild.Id = id;
                    return guild;
                }
            }
        }

        public Guild Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var guild = Load(id);
                if (guild != null)
                {
                    dbConnection.Execute("DELETE FROM guild WHERE id = @id", new {id = id});
                    guild.Id = 0;
                }

                return guild;
            }
        }

        public Guild Delete(Guid guildGuid)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var guild = Load(guildGuid);
                if (guild != null)
                {
                    dbConnection.Execute("DELETE FROM guild WHERE guild_guid = @guild_guid", new {guild_guid = guildGuid});
                    guild.Id = 0;
                }

                return guild;
            }
        }
    }
}