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

        public async Task<Raid> Remove(RaidRequest request)
        {
                return _raidRepository.Delete(request.RaidId);
        }
    }
}