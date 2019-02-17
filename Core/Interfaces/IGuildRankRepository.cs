using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildRankRepository : IRepository<GuildRank>
    {
        GuildRank Load(string name, int guildId);
        GuildRank LoadMember(int memberId);
        IEnumerable<GuildRank> LoadGuild(int guildId);
        GuildRank Save(GuildRank guildRank);
        GuildRank Delete(int id);
        void RemoveGuild(int guildId);
    }
}