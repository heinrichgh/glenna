using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class MemberDiscordAccountRepository : IMemberDiscordAccountRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public MemberDiscordAccountRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<MemberDiscordAccount> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<MemberDiscordAccount>("SELECT id, discord_account_id, member_id FROM member_discord_account");
            }
        }

        public MemberDiscordAccount Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<MemberDiscordAccount>("SELECT id, discord_account_id, member_id FROM member_discord_account WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public MemberDiscordAccount Save(MemberDiscordAccount memberDiscordAccount)
        {
            if (memberDiscordAccount.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE member_discord_account 
                    SET 
                        discord_account_id = @DiscordAccountId,
                        member_id = @MemberId
                    WHERE 
                        id = @Id
                    ", memberDiscordAccount);

                    return memberDiscordAccount;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO member_discord_account (discord_account_id, member_id) 
                    VALUES (@DiscordAccountId, @MemberId)           
                    RETURNING id
                    ", memberDiscordAccount).Single();

                    memberDiscordAccount.Id = id;
                    return memberDiscordAccount;
                }
            }
        }

        public MemberDiscordAccount Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var guildRank = Load(id);
                if (guildRank != null)
                {
                    dbConnection.Execute("DELETE FROM member_discord_account WHERE id = @id", new {id = id});
                    guildRank.Id = 0;
                }

                return guildRank;
            }
        }
        public void RemoveMember(int memberId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var response = dbConnection.Execute("DELETE FROM member_discord_account WHERE member_id = @MemberId", new {MemberId = memberId});
            }
        }
    }
}