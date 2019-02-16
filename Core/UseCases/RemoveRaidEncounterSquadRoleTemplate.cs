using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaidEncounterSquadRoleTemplate
    {
        private readonly IRaidEncounterSquadRoleTemplateRepository _raidEncounterSquadRoleTemplateRepository;

        public RemoveRaidEncounterSquadRoleTemplate(IRaidEncounterSquadRoleTemplateRepository raidEncounterSquadRoleTemplateRepository)
        {
            _raidEncounterSquadRoleTemplateRepository = raidEncounterSquadRoleTemplateRepository;
        }

        public class RemoveRaidEncounterSquadRoleTemplateRequest
        {
            public int RaidEncounterSquadRoleTemplateId { get; set; }
        }

        public class RemoveRaidEncounterSquadRoleTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterSquadRoleTemplate RemovedRaidEncounterSquadRoleTemplate { get; set; }
        }

        public async Task<RemoveRaidEncounterSquadRoleTemplateResponse> Remove(RemoveRaidEncounterSquadRoleTemplateRequest request)
        {
            RemoveRaidEncounterSquadRoleTemplateResponse response = new RemoveRaidEncounterSquadRoleTemplateResponse();
            if (_raidEncounterSquadRoleTemplateRepository.Load(request.RaidEncounterSquadRoleTemplateId) == null)
            {
                response.Response = $"Unable to remove ID: {request.RaidEncounterSquadRoleTemplateId}";
                response.Success = false;
                return response;                
            }
            else
            {
                var removedRaidEncounter = _raidEncounterSquadRoleTemplateRepository.Delete(request.RaidEncounterSquadRoleTemplateId);
                response.Response = $"Successfully remove ID: {request.RaidEncounterSquadRoleTemplateId}";
                response.Success = true;
                response.RemovedRaidEncounterSquadRoleTemplate = removedRaidEncounter;
                return response;
            }
        }
    }
}