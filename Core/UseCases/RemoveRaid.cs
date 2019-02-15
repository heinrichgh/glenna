using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaid
    {
        private readonly IRaidRepository _raidRepository;

        public RemoveRaid(IRaidRepository raidRepository)
        {
            _raidRepository = raidRepository;
        }

        public class RaidRequest
        {
            public int RaidId { get; set; }
        }

        public class RemoveRaidResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Raid RemovedRaid { get; set; }
        }

        public async Task<RemoveRaidResponse> Remove(RaidRequest request)
        {
            if (_raidRepository.Load(request.RaidId) != null)
            {
                return new RemoveRaidResponse { Response = "Removed", Success = true, RemovedRaid = _raidRepository.Delete(request.RaidId) };
            }
            else
            {
                return new RemoveRaidResponse { Response = "Failed: " + request.RaidId + " not found", Success = false };
            }
        }
    }
}