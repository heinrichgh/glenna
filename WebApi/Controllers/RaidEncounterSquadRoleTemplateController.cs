using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddRaidEncounterSquadRoleTemplate;
using static Core.UseCases.RemoveRaidEncounterSquadRoleTemplate;

namespace WebApi.Controllers
{
    [Route("api/template/raid/encounter/squad/role")]
    [ApiController]
    public class RaidEncounterSquadRoleTemplateController : ControllerBase
    {
        public readonly IRaidEncounterSquadRoleTemplateRepository _raidEncounterSquadRoleTemplate;
        public readonly AddRaidEncounterSquadRoleTemplate _addRaidencounterSquadRoleTemplate;
        public readonly RemoveRaidEncounterSquadRoleTemplate _removeRaidEncounterSquadRoleTemplate;


        public RaidEncounterSquadRoleTemplateController(RemoveRaidEncounterSquadRoleTemplate removeRaidEncounterSquadRoleTemplate, IRaidEncounterSquadRoleTemplateRepository raidEncounterSquadRoleTemplate, AddRaidEncounterSquadRoleTemplate addRaidencounterSquadRoleTemplate)
        {
            _addRaidencounterSquadRoleTemplate = addRaidencounterSquadRoleTemplate;
            _raidEncounterSquadRoleTemplate = raidEncounterSquadRoleTemplate;
            _removeRaidEncounterSquadRoleTemplate = removeRaidEncounterSquadRoleTemplate;
        }

        [HttpGet]
        public IEnumerable<RaidEncounterSquadRoleTemplate> Index()
        {
            return _raidEncounterSquadRoleTemplate.LoadAll();
        }

        [HttpPut]
        public async Task<AddRaidEncounterSquadRoleTemplateResponse> AddEncounter(int raidEncounterSquadTemplateId, int raidRoleId)
        {
            
            return await _addRaidencounterSquadRoleTemplate.AddRaidSquadRoleTemplate(new AddRaidEncounterSquadRoleTemplate.RaidEncounterSquadRoleTemplateRequest
            {
                RaidEncounterSquadTemplateId = raidEncounterSquadTemplateId,
                RaidRoleId = raidRoleId
            });
        }

        [HttpDelete]
        public async Task<RemoveRaidEncounterSquadRoleTemplateResponse> Remove(int raidEncounterSquadRoleTemplateId)
        {
            return await _removeRaidEncounterSquadRoleTemplate.Remove(new RemoveRaidEncounterSquadRoleTemplate.RemoveRaidEncounterSquadRoleTemplateRequest
            {
                RaidEncounterSquadRoleTemplateId = raidEncounterSquadRoleTemplateId
            });
        }
    }
}