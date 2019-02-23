using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounterSquadRoleRestriction
    {
        private readonly IRaidEncounterSquadRoleRestrictionRepository _raidEncounterSquadRoleRestrictionRepository;
        private readonly IRaidEncounterSquadRoleRepository _raidEncounterSquadRoleRepository;
        private readonly IProfessionRepository _professionRepository;
        private readonly IGuildRankRepository _guildRankRepository;
        public AddRaidEncounterSquadRoleRestriction(IGuildRankRepository guildRankRepository, IProfessionRepository professionRepository,  IRaidEncounterSquadRoleRepository raidEncounterSquadRoleRepository, IRaidEncounterSquadRoleRestrictionRepository raidEncounterSquadRoleRestrictionRepository)
        {
            _raidEncounterSquadRoleRestrictionRepository = raidEncounterSquadRoleRestrictionRepository;
            _raidEncounterSquadRoleRepository = raidEncounterSquadRoleRepository;
            _professionRepository = professionRepository;
            _guildRankRepository = guildRankRepository;
        }

        public class RaidEncounterSquadRoleRestrictionRequest
        {
            public int RaidEncounterSquadRoleId { get; set; }
            public int ProfessionId { get; set; }
            public int MinimumGuildRankId { get; set; }
        }
        
        public class AddRaidEncounterSquadRoleRestrictionResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterSquadRoleRestriction SavedRaidEncounterSquadRoleRestriction { get; set; }
        }

        public async Task<AddRaidEncounterSquadRoleRestrictionResponse> AddRaidSquadRoleRestiction(RaidEncounterSquadRoleRestrictionRequest request)
        {
            AddRaidEncounterSquadRoleRestrictionResponse response = new AddRaidEncounterSquadRoleRestrictionResponse();
            var raidEncounterSquadRole = _raidEncounterSquadRoleRepository.Load(request.RaidEncounterSquadRoleId);
            if (raidEncounterSquadRole == null)
            {
                response.Response = "Raid Encounter does not exist";
                response.Success = false;
            }
            else
            {
                var profession = _professionRepository.Load(request.ProfessionId);
                if (profession == null)
                {
                    response.Response = "Profession does not exist";
                    response.Success = false;
                }
                else
                {
                    var savedRaidEncounterSquadRoleRestriction = _raidEncounterSquadRoleRestrictionRepository.Save(new RaidEncounterSquadRoleRestriction {
                        MinimumGuildRankId = request.MinimumGuildRankId,
                        ProfessionId = request.ProfessionId,
                        RaidEncounterSquadRoleId = request.RaidEncounterSquadRoleId
                    });
                    response.SavedRaidEncounterSquadRoleRestriction = savedRaidEncounterSquadRoleRestriction;
                    response.Response = "created";
                    response.Success = true;
                }
            }
            return response;
        }
    }
}