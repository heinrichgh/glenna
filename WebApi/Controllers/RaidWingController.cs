using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raid-wing")]
    [ApiController]
    public class RaidWingController : ControllerBase
    {
        private readonly IRaidWingRepository _raidWingRepository;
        private readonly IGuildWarsApi _guildWarsApi;

        public RaidWingController(IRaidWingRepository raidWingRepository, IGuildWarsApi guildWarsApi)
        {
            _raidWingRepository = raidWingRepository;
            _guildWarsApi = guildWarsApi;
        }

        [HttpGet]
        public IEnumerable<RaidWing> Index()
        {
            return _raidWingRepository.LoadAll();
        }

        [HttpGet("{apiKey}")]
        public async Task<Account> Test(string apiKey)
        {
            return await _guildWarsApi.Fetch(apiKey);
        }
    }
}