using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddRaidEncounterSquadRole
    {
        private readonly IRaidEncounterSquadRepository _raidEncounterSquadRepository;
        private readonly IRaidEncounterSquadRoleRepository _raidEncounterSquadRoleRepository;
        private readonly IRaidRoleRepository _raidRoleRepository;

        public AddRaidEncounterSquadRole(IRaidRoleRepository raidRoleRepository, IRaidEncounterSquadRepository raidEncounterSquadRepository, IRaidEncounterSquadRoleRepository raidEncounterSquadRoleRepository)
        {
            _raidEncounterSquadRoleRepository = raidEncounterSquadRoleRepository;
            _raidEncounterSquadRepository = raidEncounterSquadRepository;
            _raidRoleRepository = raidRoleRepository;
        }

        public class RaidEncounterSquadRoleRequest
        {
            public int RaidEncounterSquadId { get; set; }
            public int RaidRoleId { get; set; }
        }
        
        public class AddRaidEncounterSquadRoleResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidEncounterSquadRole SavedRaidEncounterSquadRole { get; set; }
        }

        public async Task<AddRaidEncounterSquadRoleResponse> AddRaidSquadRole(RaidEncounterSquadRoleRequest request)
        {
            AddRaidEncounterSquadRoleResponse response = new AddRaidEncounterSquadRoleResponse();
            var raidEncounterSquad = _raidEncounterSquadRepository.Load(request.RaidEncounterSquadId);
            if (raidEncounterSquad == null)
            {
                response.Response = "Raid Encounter does not exist";
                response.Success = false;
            }
            else
            {
                var existingRole = _raidRoleRepository.Load(request.RaidRoleId);
                if (existingRole == null)
                {
                    response.Response = "Raid role does not exist";
                    response.Success = false;
                }
                else
                {
                    
                    var savedRaidEncounterSquadRole = _raidEncounterSquadRoleRepository.Save(new RaidEncounterSquadRole {
                        RaidEncounterSquadId = request.RaidEncounterSquadId,
                        RaidRoleId = request.RaidRoleId
                    });
                    response.SavedRaidEncounterSquadRole = savedRaidEncounterSquadRole;
                    response.Response = "created";
                    response.Success = true;
                }
            }
            return response;
        }
    }
}