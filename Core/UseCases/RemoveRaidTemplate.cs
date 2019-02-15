using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaidTemplate
    {
        private readonly IRaidTemplateRepository _raidTempalteRepository;

        public RemoveRaidTemplate(IRaidTemplateRepository raidTempalteRepository)
        {
            _raidTempalteRepository = raidTempalteRepository;
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
            if (_raidTempalteRepository.Load(request.RaidTemplateId) != null)
            {
                return new RemoveRaidTemplateResponse { Response = "Removed", Success = true, RemovedRaidTemplate = _raidTempalteRepository.Delete(request.RaidTemplateId) };                
            }
            else
            {
                return new RemoveRaidTemplateResponse { Response = "Failed: " + request.RaidTemplateId + " not found", Success = false };
            }
        }
    }
}