using System;
using System.Threading.Tasks;
using Core.Entities.GuildWars;

namespace Core.Interfaces
{
    public interface IGuildWarsApi
    {
        Task<Account> FetchAccount(string apiKey);
        Task<Guild> FetchGuild(string apiKey, Guid guildGuid);
    }
}