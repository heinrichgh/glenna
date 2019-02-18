using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidRepository : IRaidRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<Raid> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Raid>("SELECT id, guild_id, raid_time, is_completed, created_by, state, date_created FROM raid");
            }
        }

        public Raid Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Raid>("SELECT id, guild_id, raid_time, is_completed, created_by, state, date_created FROM raid WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public IEnumerable<Raid> LoadGuildRaids(int guildId)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Raid>("SELECT id, guild_id, raid_time, is_completed, created_by, state, date_created FROM raid WHERE guild_id = @GuildId", new {GuildId = guildId});
            }
        }

        public Raid Save(Raid raid)
        {
            if (raid.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid 
                    SET 
                        guild_id = @GuildId,
                        raid_time = @RaidTime,
                        is_completed = @IsCompleted,
                        created_by = @CreatedBy,
                        state = @State,
                        date_created = @DateCreated
                    WHERE 
                        id = @Id
                    ", raid);

                    return raid;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    raid.DateCreated = DateTime.Now;
                    
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid (guild_id, raid_time, is_completed, created_by, state, date_created) 
                    VALUES (@GuildId, @RaidTime, @IsCompleted, @CreatedBy, @State, @DateCreated)           
                    RETURNING id         
                    ", raid).Single();

                    raid.Id = id;
                    return raid;
                }
            }
        }

        public Raid Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raid = Load(id);
                if (raid != null)
                {
                    dbConnection.Execute("DELETE FROM raid WHERE id = @id", new {id = id});
                    raid.Id = 0;
                }

                return raid;
            }
        }
    }
}