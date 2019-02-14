using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

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

        public RaidController(CreateRaid createRaid, CreateUser createUser, RemoveRaid removeRaid, IGuildRepository guildRepository, IRaidRepository raidRepository, IUserRepository userRepository)
        {
            _raidRepository = raidRepository;
            _userRepository = userRepository;
            _guildRepository = guildRepository;
            _createUser = createUser;
            _createRaid = createRaid;
            _removeRaid = removeRaid;
        }

        [HttpGet]
        public IEnumerable<Raid> Index()
        {
            return _raidRepository.LoadAll();
        }

        [HttpPost]
        public async Task<Raid> Create(string apiKey, string disdcordIdentity, DateTime raidTime, int raidTemplateId, int guildId)
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
        public async Task<Raid> Remove(int raidId)
        {
            return await _removeRaid.Remove(new RemoveRaid.RaidRequest {
                RaidId = raidId
            });
        }
    }
}