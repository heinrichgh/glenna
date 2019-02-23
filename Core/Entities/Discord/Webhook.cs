using System;
using System.Collections.Generic;

namespace Core.Entities.Discord
{
    public class Webhook
    {
        public string Name { get; set; }
        public string ChannelId { get; set; }
        public string Token { get; set; }
        public string Avatar { get; set; }
        public string GuildId { get; set; }
        public string Id { get; set; }
    }
}