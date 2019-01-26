using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;

namespace Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public UserRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }
        
        public IEnumerable<Member> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var value = dbConnection.Query<Member>("SELECT id, display_name, game_guid, is_commander, api_key, created_at FROM member");
                return value;
            }
        }

        public Member Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Member>("SELECT id, display_name, game_guid, is_commander, api_key, created_at FROM member WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public Member Load(Guid gameGuid)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var value = dbConnection.Query<Member>("SELECT id, display_name, game_guid, is_commander, api_key, created_at FROM member WHERE game_guid = @GameGuid", new {GameGuid = gameGuid}).FirstOrDefault();
                return value;
            }
        }

        public Member LoadByApiKey(string apiKey)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Member>("SELECT id, display_name, game_guid, is_commander, api_key, created_at FROM member WHERE api_key = @ApiKey", new {ApiKey = apiKey}).FirstOrDefault();
            }
        }

        public Member Save(Member guildwarsAccount)
        {
            if (guildwarsAccount.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    guildwarsAccount.CreatedAt = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE member 
                    SET 
                        game_guid = @GameGuid,
                        display_name = @DisplayName,
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
                    INSERT INTO member (game_guid, display_name, is_commander, api_key, created_at) 
                    VALUES (@GameGuid, @DisplayName, @IsCommander, @ApiKey, @CreatedAt)           
                    RETURNING id         
                    ", guildwarsAccount).Single();

                    guildwarsAccount.Id = id;
                    return guildwarsAccount;
                }
            }
        }
        public Member Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var account = Load(id);
                dbConnection.Execute("DELETE FROM member WHERE id = @id", new {id = id});
                account.Id = 0;
                return account;
            }
        }

        public Member Delete(Guid gameGuid)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection(false))
            {
                var account = Load(gameGuid);
                if (account != null)
                {
                    dbConnection.Execute("DELETE FROM member WHERE game_guid = @game_guid", new {game_guid = gameGuid});
                    account.Id = 0;
                }

                return account;
            }
        }
    }
}