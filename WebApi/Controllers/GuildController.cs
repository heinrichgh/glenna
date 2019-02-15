using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.CreateGuild;
using static Core.UseCases.RemoveGuild;

namespace WebApi.Controllers
{
    [Route("api/guild")]
    [ApiController]
    public class GuildController : ControllerBase
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IGuildRepository _guildRepository;
        private readonly CreateGuild _createGuild;
        private readonly RemoveGuild _removeGuild;
        public GuildController(IGuildRepository guildRepository, IGuildWarsApi guildWarsApi, CreateGuild createGuild, RemoveGuild removeGuild)
        {
            _guildRepository = guildRepository;
            _guildWarsApi = guildWarsApi;
            _createGuild = createGuild;
            _removeGuild = removeGuild;
        }

        [HttpGet]
        public IEnumerable<Guild> Index()
        {
            return _guildRepository.LoadAll();
        }

        [HttpPost]

        public async Task<CreateGuildResponse> Create(Guid guildGuid, string apiKey)
        {
            return await _createGuild.InsertGuild(new CreateGuild.NewGuildRequest { GuildGuid = guildGuid, ApiKey = apiKey });
        }

        [HttpDelete]
        public RemoveGuildResponse Remove(Guid guildGuid)
        {
            return _removeGuild.Remove(new RemoveGuild.GuildRequest { GuildGuid = guildGuid });
        }
    }
}