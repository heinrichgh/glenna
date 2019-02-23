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
    [Route("api/raid/encounter")]
    [ApiController]
    public class RaidEncounterController : ControllerBase
    {
        private readonly AddRaidEncounter _addRaidEncounter;
        private readonly RemoveRaidEncounter _removeRaidEncounter;
        private readonly IRaidEncounterRepository _raidEncounterRepository;

        public RaidEncounterController(AddRaidEncounter addRaidEncounter, RemoveRaidEncounter removeRaidEncounter, IRaidEncounterRepository raidEncounterRepository)
        {
            _addRaidEncounter = addRaidEncounter;
            _removeRaidEncounter = removeRaidEncounter;
            _raidEncounterRepository = raidEncounterRepository;
        }

        [HttpGet]
        public IEnumerable<RaidEncounter> Index()
        {
            return _raidEncounterRepository.LoadAll();
        }

        [Route("raid/{raidId}")]
        [HttpGet]
        public IEnumerable<RaidEncounter> GetGuildRaidEncounters(int raidId)
        {
            return _raidEncounterRepository.LoadByRaidId(raidId);
        }

        [HttpPut]
        public async Task<AddRaidEncounterResponse> AddEncounter(int raidId, int raidBossId)
        {
            
            return await _addRaidEncounter.AddEncounter(new AddRaidEncounter.RaidEncounterRequest 
            {
                RaidBossId = raidBossId,
                RaidId = raidId
            });
        }

        [HttpDelete]
        public async Task<RemoveRaidEncounterResponse> Remove(int raidEncounterId)
        {
            return await _removeRaidEncounter.Remove(new RemoveRaidEncounter.RaidEncounterRequest {
                RaidEncounterId = raidEncounterId
            });
        }
    }
}