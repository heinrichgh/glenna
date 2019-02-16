using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveDiscordServer
    {
        private readonly IDiscordServerRepository _discordServerRepository;
        public RemoveDiscordServer(IDiscordServerRepository discordServerRepository)
        {
            _discordServerRepository = discordServerRepository;
        }

        public class DiscordServerRequest
        {
            public int DiscordServerId { get; set; }
        }

        public class RemoveDiscordServerResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public DiscordServer RemovedDiscordServer { get; set; }
        }

        public async Task<RemoveDiscordServerResponse> Remove(DiscordServerRequest request)
        {
            RemoveDiscordServerResponse response = new RemoveDiscordServerResponse();
            if (_discordServerRepository.Load(request.DiscordServerId) == null)
            {
                response.Response = $"Not found ID: {request.DiscordServerId}";
                response.Success = false;
            }
            else
            {
                response.RemovedDiscordServer = _discordServerRepository.Delete(request.DiscordServerId);
                response.Response = $"Removed ID: {request.DiscordServerId}";
                response.Success = true;
            }
            return response;
        }
    }
}