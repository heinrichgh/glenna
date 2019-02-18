using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDiscordAccountRepository : IRepository<DiscordAccount>
    {
        DiscordAccount Load(string discordAccountIdentity);
        DiscordAccount LoadUser(int userId);
        DiscordAccount Save(DiscordAccount discordAccount);
        DiscordAccount Delete(int id);
    }
}