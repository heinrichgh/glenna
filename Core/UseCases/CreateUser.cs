using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateUser
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IUserRepository _userRepository;

        public CreateUser(IGuildWarsApi guildWarsApi, IUserRepository userRepository)
        {
            _guildWarsApi = guildWarsApi;
            _userRepository = userRepository;
        }

        public class UserRequest
        {
            public string ApiKey { get; set; }
        }

        public async Task<Member> SignUp(UserRequest request)
        {
            var user = await _guildWarsApi.FetchAccount(request.ApiKey);
            
            if (_userRepository.Load(user.Id) == null)
            {
                var savedAccount =_userRepository.Save(new Member
                {
                    ApiKey = request.ApiKey,
                    GameGuid = user.Id,
                    DisplayName = user.name,
                    IsCommander = user.Commander,
                    CreatedAt = DateTime.Now
                });

                return savedAccount;
            }

            return null;
        }
    }
}