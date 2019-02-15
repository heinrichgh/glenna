using System;

namespace Core.Entities
{
    public class GuildMember
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public int GuildRankId { get; set; }
        public int GuildwarsAccountId { get; set; }
        public DateTime? DateJoined { get; set; }
    }
}