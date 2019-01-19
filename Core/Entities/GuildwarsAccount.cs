using System;

namespace Core.Entities
{
    public class GuildwarsAccount
    {
        public int Id { get; set; }
        public Guid GameGuid { get; set; }
        public bool IsCommander { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}