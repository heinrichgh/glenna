using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateGuild
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IGuildRepository _guildRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGuildRankRepository _guildRankRepository;
        private readonly IGuildMemberRepository _guildMemberRepository;

        public CreateGuild(IGuildWarsApi guildWarsApi, IGuildRepository guildRepository, IGuildMemberRepository guildMemberRepository, IUserRepository userRepository, IGuildRankRepository guildRankRepository)
        {
            _guildWarsApi = guildWarsApi;
            _guildRepository = guildRepository;
            _guildRankRepository = guildRankRepository;
            _guildMemberRepository = guildMemberRepository;
            _userRepository = userRepository;
        }

        public class NewGuildRequest
        {
            public string ApiKey { get; set; }
            public Guid GuildGuid { get; set; }
        }

        public class CreateGuildResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Guild SavedGuild { get; set; }
            public List<GuildRank> SavedRanks { get; set; }
        }

        public async Task<CreateGuildResponse> InsertGuild(NewGuildRequest request)
        {
            CreateGuildResponse response = new CreateGuildResponse();
            if (request.ApiKey == null || request.GuildGuid == null)
            {
                response.Response = "ApiKey and GuildGuid may not be empty";
                response.Success = false;
            }
            else
            {
                var user = await _guildWarsApi.FetchAccount(request.ApiKey);
                if (user.GuildLeader.Contains(request.GuildGuid))
                {
                    var guild = await _guildWarsApi.FetchGuild(request.ApiKey, request.GuildGuid);
                    var savedGuild = new Guild();
                    var existingGuild = _guildRepository.Load(request.GuildGuid);
                    if (existingGuild == null)
                    {
                        savedGuild =_guildRepository.Save(new Guild
                        {
                            Name = guild.Name,
                            Tag = guild.Tag,
                            GuildLeader = _userRepository.Load(user.Id).Id,
                            GuildGuid = guild.Id,
                            CreatedAt = DateTime.Now,
                        });
                    }
                    else
                    {
                        savedGuild =_guildRepository.Save(new Guild
                        {
                            Id = existingGuild.Id,
                            Name = guild.Name,
                            Tag = guild.Tag,
                            GuildLeader = _userRepository.Load(user.Id).Id,
                            GuildGuid = guild.Id,
                            CreatedAt = DateTime.Now,
                        });
                    }
                        
                    
                    foreach (var rank in await _guildWarsApi.FetchGuildRanks(request.ApiKey, request.GuildGuid))
                    {
                        var existingRank = _guildRankRepository.Load(rank.Id, savedGuild.Id);
                        var savedRank = new GuildRank();
                        if (existingRank == null)
                        {
                            savedRank = _guildRankRepository.Save(new GuildRank
                            {
                                GuildId = savedGuild.Id,
                                Name = rank.Id,
                                OrderBy = rank.Order
                            });
                        }
                        else
                        {
                            savedRank = _guildRankRepository.Save(new GuildRank
                            {
                                Id = existingRank.Id,
                                GuildId = savedGuild.Id,
                                Name = rank.Id,
                                OrderBy = rank.Order
                            });
                        }
                        // response.SavedRanks.Add(savedRank);
                        
                    }
                    foreach (var member in await _guildWarsApi.FetchGuildMembers(request.ApiKey, request.GuildGuid))
                    {
                        var existingUser = _userRepository.Load(member.name);
                        var savedMember = new Member();
                        var savedGuildMember = new GuildMember();
                        if (existingUser == null)
                        {
                            response.Response = "Created placeholder account";
                            savedMember = _userRepository.Save(new Member 
                            {
                                DisplayName = member.name
                            });
                            existingUser = savedMember;
                        }
                        int rankId = _guildRankRepository.Load(member.rank, savedGuild.Id).Id;
                        var existingGuildMember = _guildMemberRepository.Load(existingUser.Id, savedGuild.Id);
                        if (existingGuildMember == null)
                        {
                            savedGuildMember = _guildMemberRepository.Save(new GuildMember
                            {
                                GuildId = savedGuild.Id,
                                GuildRankId = rankId,
                                GuildwarsAccountId = existingUser.Id,
                                DateJoined = DateTime.Now
                            });
                        }
                        else
                        {
                            savedGuildMember = _guildMemberRepository.Save(new GuildMember
                            {
                                Id = existingGuildMember.Id,
                                GuildId = savedGuild.Id,
                                GuildRankId = rankId,
                                GuildwarsAccountId = existingUser.Id,
                                DateJoined = DateTime.Now
                            });
                        }
                        
                        // response.SavedRanks.Add(savedRank);
                    }

                    response.SavedGuild = savedGuild;
                    response.Response += "Done";
                    response.Success = true;
                    
                }
                else
                {
                    response.Response = "ApiKey must belong to a guild leader";
                    response.Success = false;
                }
            }
            return response;
        }
    }
}