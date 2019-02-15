using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.CreateUser;
using static Core.UseCases.RemoveUser;

namespace WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IUserRepository _userRepository;
        private readonly CreateUser _createUser;
        private readonly RemoveUser _removeUser;
        public UserController(IUserRepository userRepository, IGuildWarsApi guildWarsApi, CreateUser createUser, RemoveUser removeUser)
        {
            _userRepository = userRepository;
            _guildWarsApi = guildWarsApi;
            _createUser = createUser;
            _removeUser = removeUser;
        }

        [HttpGet]
        public IEnumerable<Member> Index()
        {
            return _userRepository.LoadAll();
        }

        [HttpPost]

        public async Task<CreateUserResponse> Create(string apiKey)
        {
            return await _createUser.SignUp(new CreateUser.UserRequest { ApiKey = apiKey });
        }

        [HttpDelete]
        public RemoveUserResponse Remove(Guid gameGuid)
        {
            return _removeUser.Remove(new RemoveUser.UserRequest { GameGuid = gameGuid });
        }
    }
}