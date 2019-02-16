using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class RaidRoleRepository : IRaidRoleRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public RaidRoleRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<RaidRole> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidRole>("SELECT id, name, is_visible FROM raid_role");
            }
        }

        public RaidRole Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<RaidRole>("SELECT id, name, is_visible FROM raid_role WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public RaidRole Save(RaidRole raidRole)
        {
            if (raidRole.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE raid_role 
                    SET 
                        name = @Name,
                        is_visible = @IsVisible,
                    WHERE 
                        id = @Id
                    ", raidRole);

                    return raidRole;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO raid_role (name, is_visible) 
                    VALUES (@Name, @IsVisible)           
                    RETURNING id         
                    ", raidRole).Single();

                    raidRole.Id = id;
                    return raidRole;
                }
            }
        }

        public RaidRole Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var raidRole = Load(id);
                if (raidRole != null)
                {
                    dbConnection.Execute("DELETE FROM raid_role WHERE id = @id", new {id = id});
                    raidRole.Id = 0;
                }

                return raidRole;
            }
        }
    }
}