using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raid-wing")]
    [ApiController]
    public class RaidWingController : ControllerBase
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IRepository<RaidWing> _raidWingRepository;

        public RaidWingController(IRepository<RaidWing> raidWingRepository)
        {
            _raidWingRepository = raidWingRepository;
        }

        [HttpGet]
        public IEnumerable<RaidWing> Index()
        {
            return _raidWingRepository.LoadAll();
        }
    }
}