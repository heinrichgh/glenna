using System;

namespace Core.Entities
{
    public class GuildwarsAccount
    {
        public uint Id { get; set; }
        public string GameGuid { get; set; }

        public bool IsCommander { get; set; }

        public string ApiKey { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}