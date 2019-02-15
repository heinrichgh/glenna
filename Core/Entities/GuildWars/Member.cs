using System;
using System.Collections.Generic;

namespace Core.Entities.GuildWars
{
    public class Member
    {
        public string name { get; set; }
        public string rank { get; set; }
        public DateTime? joined { get; set; }
    }
}