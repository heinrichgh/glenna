using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using static Core.UseCases.CreateGuild;

namespace Core.UseCases
{
    public class CreateUser
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IUserRepository _userRepository;
        private readonly CreateGuild _createGuild;

        public CreateUser(IGuildWarsApi guildWarsApi, CreateGuild createGuild, IUserRepository userRepository)
        {
            _guildWarsApi = guildWarsApi;
            _userRepository = userRepository;
            _createGuild = createGuild;
        }

        public class UserRequest
        {
            public string ApiKey { get; set; }
        }

        public class CreateUserResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Member SavedMember { get; set; }
            public List<CreateGuildResponse> SavedGuilds { get; set; }
        }

        public async Task<CreateUserResponse> SignUp(UserRequest request)
        {
            if (request.ApiKey == null)
            {
                return new CreateUserResponse { Response = "Failed: no API-Key provided", Success = false};
            }
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
                CreateUserResponse myresponse = new CreateUserResponse();
                myresponse.SavedMember = savedAccount;

                foreach (Guid guild in user.GuildLeader)
                {
                    var createdGuild = await _createGuild.Create(new CreateGuild.NewGuildRequest {
                        ApiKey = request.ApiKey,
                        GuildGuid = guild
                    });
                    // myresponse.SavedGuilds.Add(createdGuild);
                }
                return myresponse;
            }
            return new CreateUserResponse { Response = "Failed: " + user.Id + " already exists.", Success = false};
        }
    }
}