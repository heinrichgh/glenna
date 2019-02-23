using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class ProfessionRepository : IProfessionRepository
    {
        private readonly PostgresDatabaseInterface _postgresDatabaseInterface;

        public ProfessionRepository(PostgresDatabaseInterface postgresDatabaseInterface)
        {
            _postgresDatabaseInterface = postgresDatabaseInterface;
        }

        public IEnumerable<Profession> LoadAll()
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Profession>("SELECT id, name, short_name, emoji FROM profession");
            }
        }

        public Profession Load(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                return dbConnection.Query<Profession>("SELECT id, name, short_name, emoji FROM profession WHERE id = @Id", new {Id = id}).FirstOrDefault();
            }
        }

        public Profession Save(Profession profession)
        {
            if (profession.Id != 0)
            {
                // Update
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {                    
                    var id = dbConnection.Query<int>(@"
                    UPDATE profession 
                    SET 
                        name = @Name,
                        short_name = @ShortName,
                        emoji = @Emoji
                    WHERE 
                        id = @Id
                    ", profession);

                    return profession;
                }
            }
            else
            {
                // Insert
                using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
                {
                    var id = dbConnection.Query<int>(@"
                    INSERT INTO profession (name, short_name, emoji) 
                    VALUES (@Name, @ShortName, @Emoji)           
                    RETURNING id         
                    ", profession).Single();

                    profession.Id = id;
                    return profession;
                }
            }
        }

        public Profession Delete(int id)
        {
            using (var dbConnection = _postgresDatabaseInterface.OpenConnection())
            {
                var profession = Load(id);
                if (profession != null)
                {
                    dbConnection.Execute("DELETE FROM profession WHERE id = @id", new {id = id});
                    profession.Id = 0;
                }

                return profession;
            }
        }
    }
}