using System;
using System.Collections.Generic;

namespace Core.Entities.GuildWars
{
    public class Guild
    {
        public string Motd { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
    }
}