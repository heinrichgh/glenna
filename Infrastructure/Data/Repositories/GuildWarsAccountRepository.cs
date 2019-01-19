using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;

namespace Infrastructure.Data
{
    public class GuildWarsAccountRepository : IGuildWarsAccountRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public GuildWarsAccountRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }
        
        public IEnumerable<GuildwarsAccount> LoadAll()
        {
            throw new System.NotImplementedException();
        }

        public GuildwarsAccount Load(int id)
        {
            throw new System.NotImplementedException();
        }

        public GuildwarsAccount Save(GuildwarsAccount guildwarsAccount)
        {
            if (guildwarsAccount.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    guildwarsAccount.CreatedAt = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE guildwars_account 
                    SET 
                        game_guid = @GameGuid,
                        is_commander = @IsCommander,
                        api_key = @ApiKey
                    WHERE 
                        id = @Id
                    ", guildwarsAccount).Single();

                    return guildwarsAccount;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    guildwarsAccount.CreatedAt = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO guildwars_account (game_guid, is_commander, api_key, created_at) 
                    VALUES (@GameGuid, @IsCommander, @ApiKey, @CreatedAt)           
                    RETURNING id         
                    ", guildwarsAccount).Single();

                    guildwarsAccount.Id = id;
                    return guildwarsAccount;
                }
            }
        }

        public GuildwarsAccount LoadByApiKey(string apiKey)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildwarsAccount>("SELECT id, game_guid, is_commander, api_key, created_at FROM guildwars_account WHERE api_key = @ApiKey", new {ApiKey = apiKey}).FirstOrDefault();
            }
        }
    }
}