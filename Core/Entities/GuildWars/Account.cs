using System;
using System.Collections.Generic;

namespace Core.Entities.GuildWars
{
    public class Account
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public List<Guid> Guilds { get; set; }
        public List<Guid> GuildLeader { get; set; }
        public bool Commander { get; set; }
    }
}