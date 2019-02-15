using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class GuildMemberRepository : IGuildMemberRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public GuildMemberRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<GuildMember> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildMember>("SELECT id, guild_id, guild_rank_id, guildwars_account_id, date_joined FROM guild_member");
            }
        }

        public GuildMember Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildMember>("SELECT id, guild_id, guild_rank_id, guildwars_account_id, date_joined FROM guild_member WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

         public GuildMember Load(int memberId, int guildId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildMember>("SELECT id, guild_id, guild_rank_id, guildwars_account_id, date_joined FROM guild_member WHERE guild_id = @GuildId AND guildwars_account_id = @MemberId", new {GuildId = guildId, MemberId = memberId}).FirstOrDefault();
            }
        }

        public GuildMember Save(GuildMember guildMember)
        {
            if (guildMember.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE guild_member 
                    SET 
                        guild_id = @GuildId,
                        guild_rank_id = @GuildRankId,
                        guildwars_account_id = @GuildwarsAccountId,
                        date_joined = @DateJoined
                    WHERE 
                        id = @Id
                    ", guildMember);

                    return guildMember;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO guild_member (guild_id, guild_rank_id, guildwars_account_id, date_joined) 
                    VALUES (@GuildId, @GuildRankId, @GuildwarsAccountId, @DateJoined)           
                    RETURNING id
                    ", guildMember).Single();

                    guildMember.Id = id;
                    return guildMember;
                }
            }
        }

        public GuildMember Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var guildRank = Load(id);
                if (guildRank != null)
                {
                    dbConnection.Execute("DELETE FROM guild_rank WHERE id = @id", new {id = id});
                    guildRank.Id = 0;
                }

                return guildRank;
            }
        }
    }
}