using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateGuild
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IGuildRepository _guildRepository;

        public CreateGuild(IGuildWarsApi guildWarsApi, IGuildRepository guildRepository)
        {
            _guildWarsApi = guildWarsApi;
            _guildRepository = guildRepository;
        }

        public class NewGuildRequest
        {
            public string ApiKey { get; set; }
            public Guid GuildGuid { get; set; }
        }
        
        public class NewGuildResponse
        {
            public Guild Guild { get; set; }
        }


        public async Task<Guild> Create(NewGuildRequest request)
        {
            var guild = await _guildWarsApi.FetchGuild(request.ApiKey, request.GuildGuid);

            var savedGuild =_guildRepository.Save(new Guild
            {
                Name = guild.Name,
                CreatedAt = System.DateTime.Now,
                Tag = guild.Tag
            });
            
            return savedGuild;
        }
    }
}