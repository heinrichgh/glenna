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
            CreateUserResponse myresponse = new CreateUserResponse();
            if (request.ApiKey == null)
            {
                myresponse.Response = "Failed: no API-Key provided";
                myresponse.Success = false;
            }
            else
            {
                var user = await _guildWarsApi.FetchAccount(request.ApiKey);
                if (user.name == null)
                {
                    myresponse.Response = "Invalid ApiKey or Incorrect permissions";
                    myresponse.Success = false;
                }
                else
                {
                    var exists = _userRepository.Load(user.name);
                    var savedAccount = new Member();
                    if (exists == null)
                    {
                        savedAccount =_userRepository.Save(new Member
                        {
                            Id = 0,
                            ApiKey = request.ApiKey,
                            GameGuid = user.Id,
                            DisplayName = user.name,
                            IsCommander = user.Commander,
                            CreatedAt = DateTime.Now
                        });
                        myresponse.Response = "Inserted Account";
                    }
                    else
                    {
                        savedAccount =_userRepository.Save(new Member
                        {
                            Id = exists.Id,
                            ApiKey = request.ApiKey,
                            GameGuid = user.Id,
                            DisplayName = user.name,
                            IsCommander = user.Commander,
                            CreatedAt = DateTime.Now
                        });
                        myresponse.Response = "Updated Account";
                    }
                    
                    myresponse.SavedMember = savedAccount;
                    myresponse.Success = true;

                    if (user.GuildLeader == null)
                    {
                        myresponse.Response += "\nAccount does not have guild permission";
                    }
                    else
                    {
                        foreach (Guid guild in user.GuildLeader)
                        {
                            var createdGuild = await _createGuild.InsertGuild(new CreateGuild.NewGuildRequest {
                                ApiKey = request.ApiKey,
                                GuildGuid = guild
                            });
                            // myresponse.SavedGuilds.Add(createdGuild);
                        }
                    }
                   
                }
            }
            return myresponse;
        }
    }
}