using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildRankRepository : IRepository<GuildRank>
    {
        GuildRank Load(string name, int guildId);
        GuildRank Save(GuildRank guildRank);
        GuildRank Delete(int id);
        void RemoveGuild(int guildId);
    }
}