using System;

namespace Core.Entities
{
    public class Raid
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public DateTime RaidTime { get; set; }
        public bool IsCompleted { get; set; }
        public int CreatedBy { get; set; }
        public int State { get; set; }
        public DateTime DateCreated { get; set; }
    }
}