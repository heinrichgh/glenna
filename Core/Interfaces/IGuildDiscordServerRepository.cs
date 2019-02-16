using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildDiscordServerRepository : IRepository<GuildDiscordServer>
    {
        GuildDiscordServer Save(GuildDiscordServer guildDiscordServer);
        GuildDiscordServer Delete(int id);
        void RemoveDiscordServer(int id);
    }
}