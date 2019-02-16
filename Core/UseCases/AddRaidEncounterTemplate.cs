using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounterTemplate
    {
        private readonly IRaidTemplateRepository _raidTemplateRepository;
        private readonly IRaidEncounterTemplateRepository _raidEncounterTemplateRepository;
        private readonly IRaidEncounterSquadTemplateRepository _raidEncounterSquadTemplateRepository;
        private readonly IRaidBossRepository _raidBossRepository;

        public AddRaidEncounterTemplate(IRaidTemplateRepository raidTemplateRepository, IRaidEncounterSquadTemplateRepository raidEncounterSquadTemplateRepository, IRaidEncounterTemplateRepository raidEncounterTemplateRepository, IRaidBossRepository raidBossRepository)
        {
            _raidTemplateRepository = raidTemplateRepository;
            _raidEncounterTemplateRepository = raidEncounterTemplateRepository;
            _raidEncounterSquadTemplateRepository = raidEncounterSquadTemplateRepository;
            _raidBossRepository = raidBossRepository;
        }

        public class RaidEncounterTemplateRequest
        {
            public int RaidBossId { get; set; }
            public int RaidTemplateId { get; set; }
        }
        
        public class AddRaidEncounterTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterTemplate SavedRaidEncounterTemplate { get; set; }
        }

        public async Task<AddRaidEncounterTemplateResponse> AddEncounter(RaidEncounterTemplateRequest request)
        {
            AddRaidEncounterTemplateResponse response = new AddRaidEncounterTemplateResponse();
            var raidTemplate = _raidTemplateRepository.Load(request.RaidTemplateId);
            if (raidTemplate == null)
            {
                response.Response = "Raid Template ID does not exist";
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
                    var existingRaidEncounter = _raidEncounterTemplateRepository.Load(request.RaidTemplateId, request.RaidBossId);
                    if (existingRaidEncounter == null)
                    {
                        var savedRaidEncounterTemplate = _raidEncounterTemplateRepository.Save(new RaidEncounterTemplate
                        {
                            RaidBossId = request.RaidBossId,
                            RaidTemplateId = request.RaidTemplateId
                        });

                        for (int i = 1; i <= 10; i++)
                        {
                            var savedRaidEncounterSquadTemplate = _raidEncounterSquadTemplateRepository.Save(new RaidEncounterSquadTemplate
                            {
                                Position = i,
                                RaidEncounterTemplateId = savedRaidEncounterTemplate.Id,
                            });
                        }

                        response.Response = "Added Encounter Template";
                        response.Success = true;
                        response.SavedRaidEncounterTemplate = savedRaidEncounterTemplate;
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