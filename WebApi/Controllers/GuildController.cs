using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/guild")]
    [ApiController]
    public class GuildController : ControllerBase
    {
        private readonly IRepository<Guild> guildRepository;

        public GuildController(IRepository<Guild> guildRepository)
        {
            this.guildRepository = guildRepository;
        }

        [HttpGet]
        public IEnumerable<Guild> Index()
        {
            return guildRepository.LoadAll();
        }

        [HttpPut]
        public IEnumerable<Guild> Create(string GuildGuid, string ApiKey, string DisdcordIdentity)
        {
            return null;
        }

        [HttpDelete]
        public IEnumerable<Guild> Remove(string GuildGuid)
        {
            return null;
        }
    }
}