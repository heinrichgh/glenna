using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDiscordServerRepository : IRepository<DiscordServer>
    {
        DiscordServer Save(DiscordServer discordServer);
        DiscordServer Delete(int id);
    }
}