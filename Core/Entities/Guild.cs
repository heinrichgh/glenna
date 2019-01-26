using System;

namespace Core.Entities
{
    public class Guild
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public Guid GuildGuid { get; set; }
        public int GuildLeader { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}