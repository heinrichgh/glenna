using System;

namespace Core.Entities
{
    public class DiscordAccount
    {
        public int Id { get; set; }
        public int DisdcordIdentity { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}