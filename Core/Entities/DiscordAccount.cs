using System;

namespace Core.Entities
{
    public class DiscordAccount
    {
        public uint Id { get; set; }
        public uint DisdcordIdentity { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}