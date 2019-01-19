using System.Threading.Tasks;
using Core.Entities.GuildWars;

namespace Core.Interfaces
{
    public interface IGuildWarsApi
    {
        Task<Account> Fetch(string apiKey);
    }
}