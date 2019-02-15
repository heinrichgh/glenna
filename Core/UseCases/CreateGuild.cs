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
        private readonly IUserRepository _userRepository;

        public CreateGuild(IGuildWarsApi guildWarsApi, IGuildRepository guildRepository, IUserRepository userRepository)
        {
            _guildWarsApi = guildWarsApi;
            _guildRepository = guildRepository;
            _userRepository = userRepository;
        }

        public class NewGuildRequest
        {
            public string ApiKey { get; set; }
            public Guid GuildGuid { get; set; }
        }

        public class CreateGuildResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Guild SavedGuild { get; set; }
        }

        public async Task<CreateGuildResponse> Create(NewGuildRequest request)
        {
            var user = await _guildWarsApi.FetchAccount(request.ApiKey);
            if (user.GuildLeader.Contains(request.GuildGuid) && _guildRepository.Load(request.GuildGuid) == null)
            {
                var guild = await _guildWarsApi.FetchGuild(request.ApiKey, request.GuildGuid);
                
                var savedGuild =_guildRepository.Save(new Guild
                {
                    Name = guild.Name,
                    Tag = guild.Tag,
                    GuildLeader = _userRepository.Load(user.Id).Id,
                    GuildGuid = guild.Id,
                    CreatedAt = DateTime.Now,
                });
                
                return new CreateGuildResponse { Response = "Created", Success = true, SavedGuild = savedGuild};
            }
            else
            {
                return new CreateGuildResponse { Response = "Failed: ", Success = false };
            }
            
        }
    }
}