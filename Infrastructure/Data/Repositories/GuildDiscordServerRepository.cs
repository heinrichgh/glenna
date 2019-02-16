using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class GuildDiscordServerRepository : IGuildDiscordServerRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public GuildDiscordServerRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<GuildDiscordServer> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildDiscordServer>("SELECT id, discord_server_id, guild_id FROM guild_discord_server");
            }
        }

        public GuildDiscordServer Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildDiscordServer>("SELECT id, discord_server_id, guild_id FROM guild_discord_server WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public GuildDiscordServer Save(GuildDiscordServer guildDiscordServer)
        {
            if (guildDiscordServer.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE guild_discord_server 
                    SET 
                        discord_server_id = @DiscordServerId,
                        guild_id = @GuildId
                    WHERE 
                        id = @Id
                    ", guildDiscordServer);

                    return guildDiscordServer;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO guild_discord_server (discord_server_id, guild_id) 
                    VALUES (@DiscordServerId, @GuildId)           
                    RETURNING id         
                    ", guildDiscordServer).Single();

                    guildDiscordServer.Id = id;
                    return guildDiscordServer;
                }
            }
        }

        public GuildDiscordServer Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var guildDiscordServer = Load(id);
                if (guildDiscordServer != null)
                {
                    dbConnection.Execute("DELETE FROM guild_discord_server WHERE id = @id", new {id = id});
                    guildDiscordServer.Id = 0;
                }

                return guildDiscordServer;
            }
        }

        public void RemoveDiscordServer(int discordServerId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                dbConnection.Execute("DELETE FROM guild_discord_server WHERE discord_server_id = @DiscordServerId", new {DiscordServerId = discordServerId});
            }
        }
    }
}