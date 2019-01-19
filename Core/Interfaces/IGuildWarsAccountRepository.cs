using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildWarsAccountRepository : IRepository<GuildwarsAccount>
    {
        GuildwarsAccount LoadByApiKey(string apiKey);
    }
}