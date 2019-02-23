using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddRaidEncounterSquadRoleRestrictionTemplate;

namespace WebApi.Controllers
{
    [Route("api/template/raid/encounter/squad/role/restriction")]
    [ApiController]
    public class RaidEncounterSquadRoleRestrictionTemplateController : ControllerBase
    {
        private readonly IRaidEncounterSquadRoleRestrictionTemplateRepository _raidEncounterSquadRoleRestrictionTemplateRepository;
        private readonly AddRaidEncounterSquadRoleRestrictionTemplate _addRaidEncounterSquadRoleRestrictionTemplate;

        public RaidEncounterSquadRoleRestrictionTemplateController(AddRaidEncounterSquadRoleRestrictionTemplate addRaidEncounterSquadRoleRestrictionTemplate, IRaidEncounterSquadRoleRestrictionTemplateRepository raidEncounterSquadRoleRestrictionTemplateRepository)
        {
            _raidEncounterSquadRoleRestrictionTemplateRepository = raidEncounterSquadRoleRestrictionTemplateRepository;
            _addRaidEncounterSquadRoleRestrictionTemplate = addRaidEncounterSquadRoleRestrictionTemplate;
        }

        [HttpGet]
        public IEnumerable<RaidEncounterSquadRoleRestrictionTemplate> Index()
        {
            return _raidEncounterSquadRoleRestrictionTemplateRepository.LoadAll();
        }

        [HttpPut]
        public async Task<AddRaidEncounterSquadRoleRestrictionTemplateResponse> AddRaidSquadRoleTemplate(int minimumGuildRankId, int professionId, int raidEncounterSquadRoleTemplateId)
        {
            
            return await _addRaidEncounterSquadRoleRestrictionTemplate.AddSquadRoleRestrictionTemplate(new RaidEncounterSquadRoleRestrictionTemplateRequest
            {
                MinimumGuildRankId = minimumGuildRankId,
                ProfessionId = professionId,
                RaidEncounterSquadRoleTemplateId = raidEncounterSquadRoleTemplateId
            });
        }

    }
}