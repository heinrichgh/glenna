using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddDiscordServer
    {
        private readonly IDiscordServerRepository _discordServerRepository;

        public AddDiscordServer(IDiscordServerRepository discordServerRepository)
        {
            _discordServerRepository = discordServerRepository;
        }

        public class DiscordServerRequest
        {
            public string DiscordServerIdentity { get; set; }
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
            var savedDiscordServer = _discordServerRepository.Save(new DiscordServer
            {
                DiscordServerIdentity = request.DiscordServerIdentity
            });
            response.Response = "Created";
            response.Success = true;
            response.SavedDiscordServer = savedDiscordServer;
            return response;
        }
    }
}