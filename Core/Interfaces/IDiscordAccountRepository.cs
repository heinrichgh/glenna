using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDiscordAccountRepository : IRepository<DiscordAccount>
    {
        DiscordAccount Save(DiscordAccount discordAccount);
        DiscordAccount Delete(int id);
    }
}