using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveRaidEncounter
    {
        private readonly IRaidEncounterRepository _raidEncounterRepository;
        private readonly IRaidEncounterSquadRepository _raidEncounterSquadRepository;

        public RemoveRaidEncounter(IRaidEncounterRepository raidEncounterRepository, IRaidEncounterSquadRepository raidEncounterSquadRepository)
        {
            _raidEncounterRepository = raidEncounterRepository;
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
        }

        public class RaidEncounterRequest
        {
            public int RaidEncounterId { get; set; }
        }

        public class RemoveRaidRequest
        {
            public int RaidId { get; set; }
        }

        public class RemoveRaidEncounterResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounter RemovedRaidEncounter { get; set; }
        }

        public async Task<RemoveRaidEncounterResponse> Remove(RaidEncounterRequest request)
        {
            RemoveRaidEncounterResponse response = new RemoveRaidEncounterResponse();
            if (_raidEncounterRepository.Load(request.RaidEncounterId) == null)
            {
                response.Response = $"Unable to remove ID: {request.RaidEncounterId}";
                response.Success = false;
                return response;                
            }
            else
            {
                _raidEncounterSquadRepository.RemoveSquad(request.RaidEncounterId);
                var removedRaidEncounter = _raidEncounterRepository.Delete(request.RaidEncounterId);
                response.Response = $"Successfully remove ID: {request.RaidEncounterId}";
                response.Success = true;
                response.RemovedRaidEncounter = removedRaidEncounter;
                return response;
            }
        }
        public async Task<RemoveRaidEncounterResponse> RemoveRaid(RemoveRaidRequest request)
        {
            RemoveRaidEncounterResponse response = new RemoveRaidEncounterResponse();
            foreach (RaidEncounter raidEncounter in _raidEncounterRepository.LoadAll())
            {
                if (raidEncounter.RaidId == request.RaidId)
                {
                    _raidEncounterSquadRepository.RemoveSquad(raidEncounter.Id);
                    response.RemovedRaidEncounter = _raidEncounterRepository.Delete(raidEncounter.Id);
                }
            }
            return response;
        }
    }
}