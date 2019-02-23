using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounterSquadRoleRestrictionTemplate
    {
        private readonly IRaidEncounterSquadRoleRestrictionTemplateRepository _raidEncounterSquadRoleRestrictionTemplateRepository;
        private readonly IRaidEncounterSquadRoleTemplateRepository _raidEncounterSquadRoleTemplateRepository;
        private readonly IProfessionRepository _professionRepository;
        private readonly IGuildRankRepository _guildRankRepository;
        public AddRaidEncounterSquadRoleRestrictionTemplate(IGuildRankRepository guildRankRepository, IProfessionRepository professionRepository,  IRaidEncounterSquadRoleTemplateRepository raidEncounterSquadRoleTemplateRepository, IRaidEncounterSquadRoleRestrictionTemplateRepository raidEncounterSquadRoleRestrictionTemplateRepository)
        {
            _raidEncounterSquadRoleRestrictionTemplateRepository = raidEncounterSquadRoleRestrictionTemplateRepository;
            _raidEncounterSquadRoleTemplateRepository = raidEncounterSquadRoleTemplateRepository;
            _professionRepository = professionRepository;
            _guildRankRepository = guildRankRepository;
        }

        public class RaidEncounterSquadRoleRestrictionTemplateRequest
        {
            public int RaidEncounterSquadRoleTemplateId { get; set; }
            public int ProfessionId { get; set; }
            public int MinimumGuildRankId { get; set; }
        }
        
        public class AddRaidEncounterSquadRoleRestrictionTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterSquadRoleRestrictionTemplate SavedRaidEncounterSquadRoleRestrictionTemplate { get; set; }
        }

        public async Task<AddRaidEncounterSquadRoleRestrictionTemplateResponse> AddSquadRoleRestrictionTemplate(RaidEncounterSquadRoleRestrictionTemplateRequest request)
        {
            AddRaidEncounterSquadRoleRestrictionTemplateResponse response = new AddRaidEncounterSquadRoleRestrictionTemplateResponse();
            var raidEncounterSquadRoleTemplate = _raidEncounterSquadRoleTemplateRepository.Load(request.RaidEncounterSquadRoleTemplateId);
            if (raidEncounterSquadRoleTemplate == null)
            {
                response.Response = "Raid Encounter does not exist";
                response.Success = false;
            }
            else
            {
                var profession = _professionRepository.Load(request.ProfessionId);
                if (profession == null)
                {
                    response.Response = "Profession does not exist";
                    response.Success = false;
                }
                else
                {
                    var savedRaidEncounterSquadRoleRestrictionTemplate = _raidEncounterSquadRoleRestrictionTemplateRepository.Save(new RaidEncounterSquadRoleRestrictionTemplate {
                        MinimumGuildRankId = request.MinimumGuildRankId,
                        ProfessionId = request.ProfessionId,
                        RaidEncounterSquadRoleTemplateId = request.RaidEncounterSquadRoleTemplateId
                    });
                    response.SavedRaidEncounterSquadRoleRestrictionTemplate = savedRaidEncounterSquadRoleRestrictionTemplate;
                    response.Response = "created";
                    response.Success = true;
                }
            }
            return response;
        }
    }
}