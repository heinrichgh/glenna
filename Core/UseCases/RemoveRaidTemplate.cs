using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaidTemplate
    {
        private readonly IRaidTemplateRepository _raidTempalteRepository;
        private readonly IRaidEncounterTemplateRepository _raidEncounterTemplateRepository;

        public RemoveRaidTemplate(IRaidTemplateRepository raidTempalteRepository, IRaidEncounterTemplateRepository raidEncounterTemplateRepository)
        {
            _raidTempalteRepository = raidTempalteRepository;
            _raidEncounterTemplateRepository = raidEncounterTemplateRepository;
        }

        public class RaidRequest
        {
            public int RaidTemplateId { get; set; }
        }

        public class RemoveRaidTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidTemplate RemovedRaidTemplate { get; set; }
        }

        public async Task<RemoveRaidTemplateResponse> Remove(RaidRequest request)
        {
            RemoveRaidTemplateResponse response = new RemoveRaidTemplateResponse();
            if (_raidTempalteRepository.Load(request.RaidTemplateId) == null)
            {
                response.Response = $"Unable to remove ID: {request.RaidTemplateId}";
                response.Success = false;
                return response;                
            }
            else
            {
                foreach (RaidEncounterTemplate raidEncounterTemplate in _raidEncounterTemplateRepository.LoadAll())
                {
                    if (raidEncounterTemplate.RaidTemplateId == request.RaidTemplateId)
                    {
                        _raidEncounterTemplateRepository.Delete(raidEncounterTemplate.Id);
                    }
                }
                var removedRaidEncounter = _raidTempalteRepository.Delete(request.RaidTemplateId);
                response.Response = $"Successfully remove ID: {request.RaidTemplateId}";
                response.Success = true;
                response.RemovedRaidTemplate = removedRaidEncounter;
                return response;
            }
        }
    }
}