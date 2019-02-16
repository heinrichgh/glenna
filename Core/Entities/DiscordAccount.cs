using System;

namespace Core.Entities
{
    public class DiscordAccount
    {
        public int Id { get; set; }
        public string DiscordIdentity { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}