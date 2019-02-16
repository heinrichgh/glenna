using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaid
    {
        private readonly IRaidRepository _raidRepository;
        private readonly RemoveRaidEncounter _removeRaidEncounter;

        public RemoveRaid(IRaidRepository raidRepository, RemoveRaidEncounter removeRaidEncounter)
        {
            _raidRepository = raidRepository;
            _removeRaidEncounter = removeRaidEncounter;
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
            RemoveRaidResponse response = new RemoveRaidResponse();
            if (_raidRepository.Load(request.RaidId) == null)
            {
                response.Response = $"Unable to remove ID: {request.RaidId}";
                response.Success = false;
                return response;                
            }
            else
            {
                var removedRaidEncounter = _removeRaidEncounter.RemoveRaid(new RemoveRaidEncounter.RemoveRaidRequest
                {
                    RaidId = request.RaidId
                });
                response.RemovedRaid = _raidRepository.Delete(request.RaidId);
                response.Response = $"Successfully remove ID: {request.RaidId}";
                response.Success = true;
                return response;
            }
        }
    }
}