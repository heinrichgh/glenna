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
    [Route("api/raid/encounter/squad")]
    [ApiController]
    public class RaidEncounterSquadController : ControllerBase
    {
        private readonly IRaidEncounterSquadRepository _raidEncounterSquadRepository;

        public RaidEncounterSquadController(IRaidEncounterSquadRepository raidEncounterSquadRepository)
        {
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
        }

        [HttpGet]
        public IEnumerable<RaidEncounterSquad> Index()
        {
            return _raidEncounterSquadRepository.LoadAll();
        }

        [Route("encounter/{encounterId}")]
        [HttpGet]
        public IEnumerable<RaidEncounterSquad> GetGuildRaidEncounters(int encounterId)
        {
            return _raidEncounterSquadRepository.LoadSquad(encounterId);
        }
    }
}