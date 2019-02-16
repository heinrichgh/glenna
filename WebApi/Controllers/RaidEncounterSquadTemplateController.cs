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
    [Route("api/template/raid/encounter/squad")]
    [ApiController]
    public class RaidEncounterSquadTemplateController : ControllerBase
    {
        private readonly IRaidEncounterSquadTemplateRepository _raidEncounterSquadRepository;

        public RaidEncounterSquadTemplateController(IRaidEncounterSquadTemplateRepository raidEncounterSquadRepository)
        {
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
        }

        [HttpGet]
        public IEnumerable<RaidEncounterSquadTemplate> Index()
        {
            return _raidEncounterSquadRepository.LoadAll();
        }
    }
}