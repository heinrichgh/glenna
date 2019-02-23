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
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly CreateUser _createUser;
        private readonly CreateRaid _createRaid;
        private readonly RemoveRaid _removeRaid;
        private readonly AddRaidEncounter _addRaidEncounter;
        private readonly IRaidEncounterRepository _raidEncounterRepository;
        private readonly IRaidBossRepository _raidBossRepository;
        private readonly IRaidEncounterSquadRepository _raidEncounterSquadRepository;
        private readonly IRaidEncounterSquadRoleRepository _raidEncounterSquadRoleRepository;
        private readonly IRaidRoleRepository _raidRoleRepository;
        private readonly IGuildMemberRepository _guildMemberRepository;
        private readonly IRaidEncounterSquadRoleRestrictionRepository _raidEncounterSquadRoleRestrictionRepository;
        private readonly IProfessionRepository _professionRepository;
        

        public class DiscordRaidResposeFields
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class DiscordRaidRespose
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public IEnumerable<DiscordRaidResposeFields> Fields { get; set; }
        }

        public RaidController(IProfessionRepository professionRepository, IRaidEncounterSquadRoleRestrictionRepository raidEncounterSquadRoleRestrictionRepository, IGuildMemberRepository guildMemberRepository, IRaidRoleRepository raidRoleRepository, IRaidEncounterSquadRoleRepository raidEncounterSquadRoleRepository, IRaidEncounterSquadRepository raidEncounterSquadRepository, IRaidBossRepository raidBossRepository, IRaidEncounterRepository raidEncounterRepository, IDiscordAccountRepository discordAccountRepository, CreateRaid createRaid, CreateUser createUser, RemoveRaid removeRaid, AddRaidEncounter addRaidEncounter, IGuildRepository guildRepository, IRaidRepository raidRepository, IUserRepository userRepository)
        {
            _raidRepository = raidRepository;
            _userRepository = userRepository;
            _guildRepository = guildRepository;
            _discordAccountRepository = discordAccountRepository;
            _createUser = createUser;
            _createRaid = createRaid;
            _removeRaid = removeRaid;
            _addRaidEncounter = addRaidEncounter;
            _raidEncounterRepository = raidEncounterRepository;
            _raidBossRepository = raidBossRepository;
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
            _raidEncounterSquadRoleRepository = raidEncounterSquadRoleRepository;
            _raidEncounterSquadRoleRestrictionRepository = raidEncounterSquadRoleRestrictionRepository;
            _raidRoleRepository = raidRoleRepository;
            _guildMemberRepository = guildMemberRepository;
            _professionRepository = professionRepository;
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

        [Route("format/{raidId}")]
        [HttpGet]
        public DiscordRaidRespose GetFormattedGuildRaids(int raidId, int discordTemplateId)
        {
            if (discordTemplateId == 0)
            {
                var raid = _raidRepository.Load(raidId);
                DiscordRaidRespose response = new DiscordRaidRespose();

                response.Title = "Raid Schedule";
                response.Url = "http://www.glenna.co.za";
                response.Description = $"\n- Unique ID:\t {raidId}";
                response.Description += $"\n- Scheduled by:\t {_discordAccountRepository.LoadUser(raid.CreatedBy).DiscordIdentity} ";
                response.Description += $"\n- Start time:\t {raid.RaidTime.ToShortDateString()} at {raid.RaidTime.ToShortTimeString()} server time";
                response.Description += $"\n- Restrictions:\t None, anyone can join";
     
                List<DiscordRaidResposeFields> fields = new List<DiscordRaidResposeFields>();
                DiscordRaidResposeFields encounterResponse = new DiscordRaidResposeFields();
                DiscordRaidResposeFields squadResponse = new DiscordRaidResposeFields();
                // // Section 2 - Encounters
                encounterResponse.Name = "Encounters:";
                squadResponse.Name = "Squad:";
                int wingId = 0;
                var raidEncounters = _raidEncounterRepository.LoadByRaidId(raid.Id);
                Dictionary<int, string> lookupSquad = new Dictionary<int, string>();
                Dictionary<int, List<string>> lookupRoles = new Dictionary<int, List<string>>();
                Dictionary<string, List<string>> lookupClass = new Dictionary<string, List<string>>();
                foreach (RaidEncounter raidEncounter in raidEncounters)
                {
                    
                    var raidboss = _raidBossRepository.Load(raidEncounter.RaidBossId);
                    if (wingId < raidboss.RaidWingId)
                    {
                        wingId = raidboss.RaidWingId;
                        encounterResponse.Value += $"\n**Wing {raidboss.RaidWingId}:**\n";
                    }
                    encounterResponse.Value += $"{raidboss.Name}, ";
                    var squads = _raidEncounterSquadRepository.LoadSquad(raidEncounter.Id);
                    foreach (RaidEncounterSquad squad in squads)
                    {
                        if (squad.GuildMemberId != 0)
                        {
                            var user = _guildMemberRepository.Load(squad.GuildMemberId);
                            var discordIdentity = _discordAccountRepository.LoadUser(user.GuildwarsAccountId);
                            if (lookupSquad.ContainsKey(squad.Position))
                            {
                                if (lookupSquad[squad.Position] != discordIdentity.DiscordIdentity)
                                {
                                    lookupSquad[squad.Position] += $", {discordIdentity.DiscordIdentity}";
                                }
                            }
                            else
                            {
                                lookupSquad.Add(squad.Position, discordIdentity.DiscordIdentity);
                            }
                        }

                        var roles = _raidEncounterSquadRoleRepository.LoadSquadRole(squad.Id);
                        foreach (RaidEncounterSquadRole role in roles)
                        {
                            var roleDescription = _raidRoleRepository.Load(role.RaidRoleId);
                            if (!lookupRoles.ContainsKey(squad.Position))
                            {
                                lookupRoles.Add(squad.Position,new List<string>());
                            }
                            if (!lookupRoles[squad.Position].Contains(roleDescription.Name))
                            {
                                lookupRoles[squad.Position].Add(roleDescription.Name);
                            }

                            var restrictions = _raidEncounterSquadRoleRestrictionRepository.LoadRoleRestrictions(role.Id);
                            foreach (RaidEncounterSquadRoleRestriction restriction in restrictions)
                            {
                                var profession = _professionRepository.Load(restriction.ProfessionId);
                                if (!lookupClass.ContainsKey(roleDescription.Name))
                                {
                                    lookupClass.Add(roleDescription.Name, new List<string>());
                                }

                                if (!lookupClass[roleDescription.Name].Contains(profession.Name))
                                {
                                    lookupClass[roleDescription.Name].Add(profession.Name);
                                }
                            }
                        }
                    }
                }
                
                for (int position = 1; position <= 10; position++)
                {
                    if (lookupSquad.ContainsKey(position))
                    {
                        squadResponse.Value += $"\n - {position}: {lookupSquad[position]}";
                    }
                    else
                    {
                        squadResponse.Value += $"\n - {position}: Open";
                    }
                    if (lookupRoles.ContainsKey(position))
                    {
                        squadResponse.Value += $" as ";
                        foreach (var role in lookupRoles[position])
                        {
                            squadResponse.Value += $"{role}";
                            if (lookupClass.ContainsKey(role))
                            {
                                squadResponse.Value += $" (";
                                foreach (var profession in lookupClass[role])
                                {
                                    squadResponse.Value += $"{profession}, ";
                                }
                                squadResponse.Value += $")";
                            }
                            else
                            {
                                squadResponse.Value += $", ";
                            }
                        }
                    }
                }
                
                fields.Add(encounterResponse);
                fields.Add(squadResponse);
                response.Fields = fields;
    
                return response;
            }
            else
            {
                // Load user designed template
                return new DiscordRaidRespose();
            }
            
        }

        [HttpPut]
        public async Task<AddRaidEncounterResponse> AddEncounter(int raidId, int raidBossId)
        {
            // do webhook https://discordapp.com/api/webhooks/547462360881823745/dDPJi8vJbxUtcMrW5Tq9LqXWAZPiyZdaVfvUjXiDBPaeLXtNad1z-fAPhIW_8hv8Hqu_
            return await _addRaidEncounter.AddEncounter(new AddRaidEncounter.RaidEncounterRequest 
            {
                RaidBossId = raidBossId,
                RaidId = raidId
            });
        }

        [HttpPost]
        public async Task<CreateRaidResponse> Create(string apiKey, string disdcordIdentity, DateTime raidTime, int raidTemplateId, int guildId)
        {
            // do webhook https://discordapp.com/api/webhooks/547462360881823745/dDPJi8vJbxUtcMrW5Tq9LqXWAZPiyZdaVfvUjXiDBPaeLXtNad1z-fAPhIW_8hv8Hqu_
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
            // do webhook https://discordapp.com/api/webhooks/547462360881823745/dDPJi8vJbxUtcMrW5Tq9LqXWAZPiyZdaVfvUjXiDBPaeLXtNad1z-fAPhIW_8hv8Hqu_
            return await _removeRaid.Remove(new RemoveRaid.RaidRequest {
                RaidId = raidId
            });
        }
    }
}