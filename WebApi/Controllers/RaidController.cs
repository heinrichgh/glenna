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

namespace WebApi.Controllers
{
    [Route("api/raid")]
    [ApiController]
    public class RaidController : ControllerBase
    {
        private readonly IRaidRepository _raidRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGuildRepository _guildRepository;
        private readonly CreateUser _createUser;
        private readonly CreateRaid _createRaid;
        private readonly RemoveRaid _removeRaid;
        private readonly AddRaidEncounter _addRaidEncounter;

        public RaidController(CreateRaid createRaid, CreateUser createUser, RemoveRaid removeRaid, AddRaidEncounter addRaidEncounter, IGuildRepository guildRepository, IRaidRepository raidRepository, IUserRepository userRepository)
        {
            _raidRepository = raidRepository;
            _userRepository = userRepository;
            _guildRepository = guildRepository;
            _createUser = createUser;
            _createRaid = createRaid;
            _removeRaid = removeRaid;
            _addRaidEncounter = addRaidEncounter;
        }

        [HttpGet]
        public IEnumerable<Raid> Index()
        {
            return _raidRepository.LoadAll();
        }

        [Route("guild/{guildId}")]
        [HttpGet]
        public IEnumerable<Raid> GetGuildRaids(int guildId)
        {
            return _raidRepository.LoadGuildRaids(guildId);
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

        [HttpPost]
        public async Task<CreateRaidResponse> Create(string apiKey, string disdcordIdentity, DateTime raidTime, int raidTemplateId, int guildId)
        {
            var user = _userRepository.LoadByApiKey(apiKey);
            if (user != null)
            {
                if (_guildRepository.Load(guildId) == null)
                {
                    guildId = 0;
                }

                return await _createRaid.Schedule(new CreateRaid.RaidRequest { 
                    CreatedBy = user.Id,
                    GuildId = guildId,
                    RaidTime = raidTime, 
                    RaidTemplateId = raidTemplateId,
                    IsCompleted = false,
                    State = 0 });
            }
            else
            {
                await _createUser.SignUp(new CreateUser.UserRequest { ApiKey = apiKey });
                return await this.Create(apiKey, disdcordIdentity, raidTime, raidTemplateId, guildId);
            }            
        }

        [HttpDelete]
        public async Task<RemoveRaidResponse> Remove(int raidId)
        {
            return await _removeRaid.Remove(new RemoveRaid.RaidRequest {
                RaidId = raidId
            });
        }
    }
}