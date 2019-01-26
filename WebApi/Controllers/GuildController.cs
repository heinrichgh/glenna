using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

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
        public GuildController(IGuildRepository guildRepository, IGuildWarsApi guildWarsApi, CreateGuild signUpGuild, RemoveGuild removeGuild)
        {
            _guildRepository = guildRepository;
            _guildWarsApi = guildWarsApi;
            _createGuild = signUpGuild;
            _removeGuild = removeGuild;
        }

        [HttpGet]
        public IEnumerable<Guild> Index()
        {
            return _guildRepository.LoadAll();
        }

        [HttpPost]

        public async Task<Guild> Create(string guildGuid, string apiKey)
        {
            return await _createGuild.Create(new CreateGuild.NewGuildRequest { ApiKey = apiKey });
        }

        [HttpDelete]
        public Guild Remove(Guid guildGuid)
        {
            return _removeGuild.Remove(new RemoveGuild.GuildRequest { GuildGuid = guildGuid });
        }
    }
}