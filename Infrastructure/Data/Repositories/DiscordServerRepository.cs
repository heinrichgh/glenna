using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class DiscordServerRepository : IDiscordServerRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public DiscordServerRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<DiscordServer> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<DiscordServer>("SELECT id, discord_server_identity FROM discord_server");
            }
        }

        public DiscordServer Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<DiscordServer>("SELECT id, discord_server_identity FROM discord_server WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public DiscordServer Save(DiscordServer discordServer)
        {
            if (discordServer.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE discord_server 
                    SET 
                        discord_server_identity = @DiscordServerIdentity
                    WHERE 
                        id = @Id
                    ", discordServer);

                    return discordServer;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO discord_server (discord_server_identity) 
                    VALUES (@DiscordServerIdentity)           
                    RETURNING id         
                    ", discordServer).Single();

                    discordServer.Id = id;
                    return discordServer;
                }
            }
        }

        public DiscordServer Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidRole = Load(id);
                if (raidRole != null)
                {
                    dbConnection.Execute("DELETE FROM discord_server WHERE id = @id", new {id = id});
                    raidRole.Id = 0;
                }

                return raidRole;
            }
        }
    }
}