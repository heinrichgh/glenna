using System;

namespace Core.Entities
{
    public class Guild
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        // public string GuildGuid { get; set; }
        // public uint GuildLeader { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}