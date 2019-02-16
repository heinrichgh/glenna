using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounterSquadRoleTemplate
    {
        private readonly IRaidEncounterSquadTemplateRepository _raidEncounterSquadTemplateRepository;
        private readonly IRaidEncounterSquadRoleTemplateRepository _raidEncounterSquadRoleTemplateRepository;
        private readonly IRaidRoleRepository _raidRoleRepository;

        public AddRaidEncounterSquadRoleTemplate(IRaidRoleRepository raidRoleRepository, IRaidEncounterSquadTemplateRepository raidEncounterSquadTemplateRepository, IRaidEncounterSquadRoleTemplateRepository raidEncounterSquadRoleTemplateRepository)
        {
            _raidEncounterSquadRoleTemplateRepository = raidEncounterSquadRoleTemplateRepository;
            _raidEncounterSquadTemplateRepository = raidEncounterSquadTemplateRepository;
            _raidRoleRepository = raidRoleRepository;
        }

        public class RaidEncounterSquadRoleTemplateRequest
        {
            public int RaidEncounterSquadTemplateId { get; set; }
            public int RaidRoleId { get; set; }
        }
        
        public class AddRaidEncounterSquadRoleTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterSquadRoleTemplate SavedRaidEncounterSquadRoleTemplate { get; set; }
        }

        public async Task<AddRaidEncounterSquadRoleTemplateResponse> AddRaidSquadRoleTemplate(RaidEncounterSquadRoleTemplateRequest request)
        {
            AddRaidEncounterSquadRoleTemplateResponse response = new AddRaidEncounterSquadRoleTemplateResponse();
            var raidEncounterSquadTemplate = _raidEncounterSquadTemplateRepository.Load(request.RaidEncounterSquadTemplateId);
            if (raidEncounterSquadTemplate == null)
            {
                response.Response = "Raid Encounter does not exist";
                response.Success = false;
            }
            else
            {
                var existingRole = _raidRoleRepository.Load(request.RaidRoleId);
                if (existingRole == null)
                {
                    response.Response = "Raid role does not exist";
                    response.Success = false;
                }
                else
                {
                    var savedRaidEncounterSquadRoleTemplate = _raidEncounterSquadRoleTemplateRepository.Save(new RaidEncounterSquadRoleTemplate {
                        RaidEncounterSquadTemplateId = request.RaidEncounterSquadTemplateId,
                        RaidRoleId = request.RaidRoleId
                    });
                    response.SavedRaidEncounterSquadRoleTemplate = savedRaidEncounterSquadRoleTemplate;
                    response.Response = "created";
                    response.Success = true;
                }
            }
            return response;
        }
    }
}