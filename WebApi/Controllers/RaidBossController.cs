using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddRaidEncounter;
using static Core.UseCases.CreateRaid;
using static Core.UseCases.RemoveRaid;
using static Core.UseCases.RemoveRaidEncounter;

namespace WebApi.Controllers
{
    [Route("api/raid/boss")]
    [ApiController]
    public class RaidBossController : ControllerBase
    {
        private readonly IRaidBossRepository _raidBossRepository;

        public RaidBossController(IRaidBossRepository raidBossRepository)
        {
            _raidBossRepository = raidBossRepository;
        }

        [HttpGet]
        public IEnumerable<RaidBoss> Index()
        {
            return _raidBossRepository.LoadAll();
        }

        [Route("raid/boss/{raidBossId}")]
        [HttpGet]
        public RaidBoss GetGuildRaidEncounters(int raidBossId)
        {
            return _raidBossRepository.Load(raidBossId);
        }
    }
}