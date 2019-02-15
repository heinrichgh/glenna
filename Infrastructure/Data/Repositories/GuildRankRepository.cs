using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class GuildRankRepository : IGuildRankRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public GuildRankRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<GuildRank> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildRank>("SELECT id, guild_id, name, order_by FROM guild_rank");
            }
        }

        public GuildRank Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildRank>("SELECT id, guild_id, name, order_by FROM guild_rank WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public GuildRank Load(string name, int guildId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<GuildRank>("SELECT id, guild_id, name, order_by FROM guild_rank WHERE name = @Name AND guild_id = @GuildId", new {Name = name, GuildId = guildId}).FirstOrDefault();
            }
        }

        public GuildRank Save(GuildRank guildRank)
        {
            if (guildRank.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE guild_rank 
                    SET 
                        guild_id = @GuildId,
                        name = @Name,
                        order_by = @OrderBy
                    WHERE 
                        id = @Id
                    ", guildRank);

                    return guildRank;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO guild_rank (guild_id, name, order_by) 
                    VALUES (@GuildId, @Name, @OrderBy)           
                    RETURNING id         
                    ", guildRank).Single();

                    guildRank.Id = id;
                    return guildRank;
                }
            }
        }

        public GuildRank Delete(int id)
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