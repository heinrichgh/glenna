using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class DiscordAccountRepository : IDiscordAccountRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public DiscordAccountRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<DiscordAccount> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<DiscordAccount>("SELECT id, discord_identity, status, created_at FROM discord_account");
            }
        }

        public DiscordAccount Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<DiscordAccount>("SELECT id, discord_identity, status, created_at FROM discord_account WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public DiscordAccount Save(DiscordAccount discordAccount)
        {
            if (discordAccount.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE discord_account 
                    SET 
                        discord_identity = @DiscordIdentity,
                        status = @Status,
                        created_at = @CreatedAt
                    WHERE 
                        id = @Id
                    ", discordAccount);

                    return discordAccount;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO discord_account (discord_identity, status, created_at) 
                    VALUES (@DiscordIdentity, @Status, @CreatedAt)           
                    RETURNING id         
                    ", discordAccount).Single();

                    discordAccount.Id = id;
                    return discordAccount;
                }
            }
        }

        public DiscordAccount Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var discordAccount = Load(id);
                if (discordAccount != null)
                {
                    dbConnection.Execute("DELETE FROM discord_account WHERE id = @id", new {id = id});
                    discordAccount.Id = 0;
                }

                return discordAccount;
            }
        }
    }
}