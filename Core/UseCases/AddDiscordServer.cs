using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddDiscordServer
    {
        private readonly IDiscordServerRepository _discordServerRepository;
        private readonly IGuildDiscordServerRepository _guildDiscordServerRepository;
        private readonly IGuildRepository _guildRepository;

        public AddDiscordServer(IGuildDiscordServerRepository guildDiscordServerRepository, IDiscordServerRepository discordServerRepository, IGuildRepository guildRepository)
        {
            _discordServerRepository = discordServerRepository;
            _guildRepository = guildRepository;
            _guildDiscordServerRepository = guildDiscordServerRepository;
        }

        public class DiscordServerRequest
        {
            public string DiscordServerIdentity { get; set; }
            public int GuildId { get; set; }
        }
        
        public class AddDiscordServerResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public DiscordServer SavedDiscordServer { get; set; }
        }

        public async Task<AddDiscordServerResponse> Add(DiscordServerRequest request)
        {
            AddDiscordServerResponse response = new AddDiscordServerResponse();
            if (_guildRepository.Load(request.GuildId) == null)
            {
                response.Response = $"Missing ID: {request.GuildId}";
                response.Success = false;
            }
            else
            {
                var savedDiscordServer = _discordServerRepository.Save(new DiscordServer
                {
                    DiscordServerIdentity = request.DiscordServerIdentity
                });
                var savedGuildDiscordServer = _guildDiscordServerRepository.Save(new GuildDiscordServer
                {
                    DiscordServerId = savedDiscordServer.Id,
                    GuildId = request.GuildId
                });
                response.Response = "Created";
                response.Success = true;
                response.SavedDiscordServer = savedDiscordServer;
            }
            
           
            return response;
        }
    }
}