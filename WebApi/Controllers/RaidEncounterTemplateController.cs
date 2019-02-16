using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddRaidEncounter;
using static Core.UseCases.AddRaidEncounterTemplate;
using static Core.UseCases.CreateRaid;
using static Core.UseCases.RemoveRaid;
using static Core.UseCases.RemoveRaidEncounter;
using static Core.UseCases.RemoveRaidEncounterTemplate;

namespace WebApi.Controllers
{
    [Route("api/template/raid/encounter")]
    [ApiController]
    public class RaidEncounterTemplateController : ControllerBase
    {
        private readonly AddRaidEncounterTemplate _addRaidEncounterTemplate;
        private readonly RemoveRaidEncounterTemplate _removeRaidEncounterTemplate;
        private readonly IRaidEncounterTemplateRepository _raidEncounterTemplateRepository;

        public RaidEncounterTemplateController(RemoveRaidEncounterTemplate removeRaidEncounterTemplate, AddRaidEncounterTemplate addRaidEncounterTemplate, IRaidEncounterTemplateRepository raidEncounterTemplateRepository)
        {
            _addRaidEncounterTemplate = addRaidEncounterTemplate;
            _removeRaidEncounterTemplate = removeRaidEncounterTemplate;
            _raidEncounterTemplateRepository = raidEncounterTemplateRepository;
        }

        [HttpGet]
        public IEnumerable<RaidEncounterTemplate> Index()
        {
            return _raidEncounterTemplateRepository.LoadAll();
        }

        [HttpPut]
        public async Task<AddRaidEncounterTemplateResponse> AddEncounter(int raidTemplateId, int raidBossId)
        {
            
            return await _addRaidEncounterTemplate.AddEncounter(new AddRaidEncounterTemplate.RaidEncounterTemplateRequest 
            {
                RaidBossId = raidBossId,
                RaidTemplateId = raidTemplateId
            });
        }

        [HttpDelete]
        public async Task<RemoveRaidEncounterTemplateResponse> Remove(int raidEncounterId)
        {
            return await _removeRaidEncounterTemplate.Remove(new RemoveRaidEncounterTemplate.RaidRequest {
                RaidEncounterTemplateId = raidEncounterId
            });
        }
    }
}