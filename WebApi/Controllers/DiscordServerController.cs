using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddDiscordServer;
using static Core.UseCases.RemoveDiscordServer;

namespace WebApi.Controllers
{
    [Route("api/discord/server")]
    [ApiController]
    public class DiscordServerController : ControllerBase
    {
        private readonly IDiscordServerRepository _discordServerRepository; 
        private readonly AddDiscordServer _addDiscordServer;
        private readonly RemoveDiscordServer _removeDiscordServer;
        public DiscordServerController(IDiscordServerRepository discordServerRepository, AddDiscordServer addDiscordServer, RemoveDiscordServer removeDiscordServer)
        {
            _discordServerRepository = discordServerRepository;
            _addDiscordServer = addDiscordServer;
            _removeDiscordServer = removeDiscordServer;
        }

        [HttpGet]
        public IEnumerable<DiscordServer> Index()
        {
            return _discordServerRepository.LoadAll();
        }

        [HttpPut]
        public async Task<AddDiscordServerResponse> Create(string discordServerIdentity, int guildId)
        {
            return await _addDiscordServer.Add(new AddDiscordServer.DiscordServerRequest
            {
                DiscordServerIdentity = discordServerIdentity
            });
        }

        [HttpDelete]
        public async Task<RemoveDiscordServerResponse> Remove(int discordServerId)
        {
            return await _removeDiscordServer.Remove(new RemoveDiscordServer.DiscordServerRequest
            {
                DiscordServerId = discordServerId
            });
        }
    }
}