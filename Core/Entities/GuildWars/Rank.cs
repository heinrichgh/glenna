using System;
using System.Collections.Generic;

namespace Core.Entities.GuildWars
{
    public class Rank
    {
        public string Id { get; set; }
        public int Order { get; set; }
        public string[] Permissions { get; set; }
        public string Icon { get; set; }
    }
}