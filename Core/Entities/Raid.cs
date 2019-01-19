using System;

namespace Core.Entities
{
    public class Raid
    {
        public uint Id { get; set; }
        public uint GuildId { get; set; }
        public DateTime RaidTime { get; set; }
        public bool IsCompleted { get; set; }
        public uint CreatedBy { get; set; }
        public uint State { get; set; }
        public DateTime DateCreated { get; set; }
    }
}