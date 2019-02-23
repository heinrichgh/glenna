using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateRaid
    {
        private readonly IUserRepository _userRepository;
        private readonly IRaidRepository _raidRepository;
        private readonly IGuildRepository _guildRepository;
        private readonly IRaidTemplateRepository _raidTemplateRepository;
        private readonly IRaidEncounterTemplateRepository _raidEncounterTemplateRepository;
        private readonly AddRaidEncounter _addRaidEncounter;
        private readonly AddRaidEncounterSquadRole _addRaidEncounterSquadRole;
        private readonly AddRaidEncounterSquadRoleRestriction _addRaidEncounterSquadRoleRestriction;
        private readonly IRaidEncounterSquadRoleRestrictionTemplateRepository _raidEncounterSquadRoleRestrictionTemplateRepository;
        private readonly IRaidEncounterSquadRepository _raidEncounterSquadRepository;
        private readonly IRaidEncounterSquadTemplateRepository _raidEncounterSquadTemplateRepository;
        private readonly IRaidEncounterSquadRoleTemplateRepository _raidEncounterSquadRoleTemplateRepository;
        

        public CreateRaid(AddRaidEncounterSquadRoleRestriction addRaidEncounterSquadRoleRestriction, IRaidEncounterSquadTemplateRepository raidEncounterSquadTemplateRepository, IRaidEncounterSquadRoleTemplateRepository raidEncounterSquadRoleTemplateRepository, AddRaidEncounterSquadRole addRaidEncounterSquadRole, IRaidEncounterSquadRoleRestrictionTemplateRepository raidEncounterSquadRoleRestrictionTemplateRepository, IRaidEncounterSquadRepository raidEncounterSquadRepository, AddRaidEncounter addRaidEncounter, IRaidEncounterTemplateRepository raidEncounterTemplateRepository, IRaidTemplateRepository raidTemplateRepository, IUserRepository userRepository, IGuildRepository guildRepository, IRaidRepository raidRepository)
        {
            _userRepository = userRepository;
            _raidRepository = raidRepository;
            _guildRepository = guildRepository;
            _raidTemplateRepository = raidTemplateRepository;
            _raidEncounterTemplateRepository = raidEncounterTemplateRepository;
            _addRaidEncounter = addRaidEncounter;
            _addRaidEncounterSquadRole = addRaidEncounterSquadRole;
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
            _raidEncounterSquadRoleTemplateRepository = raidEncounterSquadRoleTemplateRepository;
            _raidEncounterSquadTemplateRepository = raidEncounterSquadTemplateRepository;
            _addRaidEncounterSquadRoleRestriction = addRaidEncounterSquadRoleRestriction;
            _raidEncounterSquadRoleRestrictionTemplateRepository = raidEncounterSquadRoleRestrictionTemplateRepository;
        }

        public class RaidRequest
        {
            public int GuildId { get; set; }
            public DateTime RaidTime { get; set; }
            public int CreatedBy { get; set; }
            public int State { get; set; }
            public int RaidTemplateId { get; set; }
            public bool IsCompleted { get; set; }
        }
        
        public class CreateRaidResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Raid SavedRaid { get; set; }
        }

        private RaidEncounterSquad ReturnSquadPosition(IEnumerable<RaidEncounterSquad> Squads, int position)
        {
            foreach (RaidEncounterSquad member in Squads)
            {
                if (member.Position == position)
                {
                    return member;
                }
            }
            return null;
        }

        public async Task<CreateRaidResponse> Schedule(RaidRequest request)
        {
            CreateRaidResponse response = new CreateRaidResponse();
            
            if (request.RaidTemplateId != 0)
            {
                var raidTemplate = _raidTemplateRepository.Load(request.RaidTemplateId);
                if (raidTemplate == null)
                {
                    response.Response = "Raid template does not exist, not used";
                    response.Success = false;
                }
                else
                {
                    var saveRaid = _raidRepository.Save(new Raid
                    {
                        GuildId = request.GuildId,
                        RaidTime = request.RaidTime,
                        CreatedBy = request.CreatedBy,
                        State = request.State,
                        DateCreated = DateTime.Now,
                        IsCompleted = request.IsCompleted
                    });
                    response.SavedRaid = saveRaid;
                    response.Success = true;

                    var raidEncounterTemplates = _raidEncounterTemplateRepository.LoadByTemplate(raidTemplate.Id);
                    foreach (RaidEncounterTemplate raidEncounterTemplate in raidEncounterTemplates)
                    {
                        var savedEncounter = await _addRaidEncounter.AddEncounter(new AddRaidEncounter.RaidEncounterRequest
                        {
                            RaidBossId = raidEncounterTemplate.RaidBossId,
                            RaidId = saveRaid.Id
                        });

                        var raidEncounterSquadTemplates = _raidEncounterSquadTemplateRepository.LoadSquad(raidEncounterTemplate.Id);
                        foreach (RaidEncounterSquadTemplate squadTemplate in raidEncounterSquadTemplates)
                        {
                            for (int position = 1; position <= 10; position++)
                            {
                                var squadRoleTemplate = _raidEncounterSquadRoleTemplateRepository.LoadSquadRole(squadTemplate.RaidEncounterTemplateId, position);
                                if (squadRoleTemplate == null)
                                {
                                    
                                }
                                else
                                {
                                    var savedSquadRole = await _addRaidEncounterSquadRole.AddRaidSquadRole(new AddRaidEncounterSquadRole.RaidEncounterSquadRoleRequest
                                    {
                                        RaidEncounterSquadId = ReturnSquadPosition(savedEncounter.SavedSquad, position).Id,
                                        RaidRoleId = squadRoleTemplate.RaidRoleId
                                    });
                                    var squadRoleRestrictionTemplates = _raidEncounterSquadRoleRestrictionTemplateRepository.LoadByRole(squadRoleTemplate.Id);
                                    foreach (RaidEncounterSquadRoleRestrictionTemplate squadRoleRestrictionTemplate in squadRoleRestrictionTemplates)
                                    {
                                        var savedSquadRoleRestriction = await _addRaidEncounterSquadRoleRestriction.AddRaidSquadRoleRestiction(new AddRaidEncounterSquadRoleRestriction.RaidEncounterSquadRoleRestrictionRequest 
                                        {
                                            RaidEncounterSquadRoleId = savedSquadRole.SavedRaidEncounterSquadRole.Id,
                                            MinimumGuildRankId = squadRoleRestrictionTemplate.MinimumGuildRankId,
                                            ProfessionId = squadRoleRestrictionTemplate.ProfessionId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var saveRaid = _raidRepository.Save(new Raid
                {
                    GuildId = request.GuildId,
                    RaidTime = request.RaidTime,
                    CreatedBy = request.CreatedBy,
                    State = request.State,
                    DateCreated = DateTime.Now,
                    IsCompleted = request.IsCompleted
                });
                response.SavedRaid = saveRaid;
                response.Success = true;
                response.Response = "Created without template";
            }

            return response;
        }
    }
}