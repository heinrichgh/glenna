using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raid-wing")]
    [ApiController]
    public class RaidWingController : ControllerBase
    {
        private readonly IRaidWingRepository raidWingRepository;

        public RaidWingController(IRaidWingRepository raidWingRepository)
        {
            this.raidWingRepository = raidWingRepository;
        }

        [HttpGet]
        public IEnumerable<RaidWing> Index()
        {
            return raidWingRepository.LoadAll();
        }
    }
}