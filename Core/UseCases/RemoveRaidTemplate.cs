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
            public int RaidId { get; set; }
        }

        public async Task<RaidTemplate> Remove(RaidRequest request)
        {
                return _raidTempalteRepository.Delete(request.RaidId);
        }
    }
}