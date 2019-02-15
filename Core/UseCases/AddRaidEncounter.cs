using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounter
    {
        private readonly IRaidRepository _raidRepository;
        private readonly IRaidEncounterRepository _raidEncounterRepository;
        private readonly IRaidBossRepository _raidBossRepository;

        public AddRaidEncounter( IRaidRepository raidRepository, IRaidEncounterRepository raidEncounterRepository, IRaidBossRepository raidBossRepository)
        {
            _raidRepository = raidRepository;
            _raidEncounterRepository = raidEncounterRepository;
            _raidBossRepository = raidBossRepository;
        }

        public class RaidEncounterRequest
        {
            public int RaidBossId { get; set; }
            public int RaidId { get; set; }
        }
        
        public class AddRaidEncounterResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounter SavedRaidEncounter { get; set; }
        }

        public async Task<AddRaidEncounterResponse> AddEncounter(RaidEncounterRequest request)
        {
            AddRaidEncounterResponse response = new AddRaidEncounterResponse();
            var raid = _raidRepository.Load(request.RaidId);
            if (raid == null)
            {
                response.Response = "Raid ID does not exist";
                response.Success = false;
            }
            else
            {
                var existingRaidBoss = _raidBossRepository.Load(request.RaidBossId);
                if (existingRaidBoss == null)
                {
                    response.Response = "Raidboss ID does not exist";
                    response.Success = false;
                }
                else
                {
                    var existingRaidEncounter = _raidEncounterRepository.Load(request.RaidId, request.RaidBossId);
                    if (existingRaidEncounter == null)
                    {
                        var savedRaidEncounter = _raidEncounterRepository.Save(new RaidEncounter
                        {
                            RaidBossId = request.RaidBossId,
                            RaidId = request.RaidId
                        });
                        response.Response = "Added Encounter";
                        response.Success = true;
                    }
                    else
                    {
                        response.Response = "Encounter already exists";
                        response.Success = false;
                    }
                }
            }
            return response;
        }
    }
}