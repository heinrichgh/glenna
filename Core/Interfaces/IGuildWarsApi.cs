using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.GuildWars;

namespace Core.Interfaces
{
    public interface IGuildWarsApi
    {
        Task<Account> FetchAccount(string apiKey);
        Task<Guild> FetchGuild(string apiKey, Guid guildGuid);
        Task<IEnumerable<Member>> FetchGuildMembers(string apiKey, Guid guildGuid);
        Task<IEnumerable<Rank>> FetchGuildRanks(string apiKey, Guid guildGuid);
    }
}