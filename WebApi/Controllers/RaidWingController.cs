using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raid-wing")]
    [ApiController]
    public class RaidWingController : ControllerBase
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IRepository<RaidWing> _raidWingRepository;
        private readonly SignUpNewUser _signUpNewUser;
        private readonly IGuildWarsAccountRepository _guildWarsAccountRepository;

        public RaidWingController(IRepository<RaidWing> raidWingRepository, SignUpNewUser signUpNewUser, IGuildWarsAccountRepository guildWarsAccountRepository)
        {
            _raidWingRepository = raidWingRepository;
            _signUpNewUser = signUpNewUser;
            _guildWarsAccountRepository = guildWarsAccountRepository;
        }

        [HttpGet]
        public IEnumerable<RaidWing> Index()
        {
            return _raidWingRepository.LoadAll();
        }

        [HttpGet("{apiKey}")]
        public async Task<SignUpNewUser.SignUpNewUserResponse> Test(string apiKey)
        {
            return await _signUpNewUser.SignUp(new SignUpNewUser.SignUpNewUserRequest { ApiKey = apiKey});
        }
    }
}