using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaidEncounterTemplate
    {
        private readonly IRaidEncounterTemplateRepository _raidEncounterTemplateRepository;

        public RemoveRaidEncounterTemplate(IRaidEncounterTemplateRepository raidEncounterTemplateRepository)
        {
            _raidEncounterTemplateRepository = raidEncounterTemplateRepository;
        }

        public class RaidRequest
        {
            public int RaidEncounterTemplateId { get; set; }
        }

        public class RemoveRaidEncounterTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterTemplate RemovedRaidEncounterTemplate { get; set; }
        }

        public async Task<RemoveRaidEncounterTemplateResponse> Remove(RaidRequest request)
        {
            if (_raidEncounterTemplateRepository.Load(request.RaidEncounterTemplateId) != null)
            {
                return new RemoveRaidEncounterTemplateResponse { Response = "Removed", Success = true, RemovedRaidEncounterTemplate = _raidEncounterTemplateRepository.Delete(request.RaidEncounterTemplateId) };                
            }
            else
            {
                return new RemoveRaidEncounterTemplateResponse { Response = "Failed: " + request.RaidEncounterTemplateId + " not found", Success = false };
            }
        }
    }
}