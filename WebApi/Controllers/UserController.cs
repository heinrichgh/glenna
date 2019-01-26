using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IUserRepository _userRepository;
        private readonly SignUpUser _signUpUser;
        private readonly RemoveUser _removeUser;
        public UserController(IUserRepository userRepository, IGuildWarsApi guildWarsApi, SignUpUser signUpUser, RemoveUser removeUser)
        {
            _userRepository = userRepository;
            _guildWarsApi = guildWarsApi;
            _signUpUser = signUpUser;
            _removeUser = removeUser;
        }

        [HttpGet]
        public IEnumerable<Member> Index()
        {
            return _userRepository.LoadAll();
        }

        [HttpPost]

        public async Task<Member> Create(string apiKey)
        {
            return await _signUpUser.SignUp(new SignUpUser.UserRequest { ApiKey = apiKey });
        }

        [HttpDelete]
        public Member Remove(Guid gameGuid)
        {
            return _removeUser.Remove(new RemoveUser.UserRequest { GameGuid = gameGuid });
        }
    }
}