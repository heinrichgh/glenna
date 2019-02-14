using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateRaid
    {
        private readonly IUserRepository _userRepository;
        private readonly IRaidRepository _raidRepository;
        private readonly IGuildRepository _guildRepository;

        public CreateRaid(IUserRepository userRepository, IGuildRepository guildRepository, IRaidRepository raidRepository)
        {
            _userRepository = userRepository;
            _raidRepository = raidRepository;
            _guildRepository = guildRepository;
        }

        public class RaidRequest
        {
            public int GuildId { get; set; }
            public DateTime RaidTime { get; set; }
            public int CreatedBy { get; set; }
            public int State { get; set; }
            public int RaidTemplateId { get; set; }
            public bool IsCompleted { get; set; }
        }
        
        public async Task<Raid> Schedule(RaidRequest request)
        {
            
            var saveRaid = _raidRepository.Save(new Raid
            {
                GuildId = request.GuildId,
                RaidTime = request.RaidTime,
                CreatedBy = request.CreatedBy,
                State = request.State,
                DateCreated = DateTime.Now,
                IsCompleted = request.IsCompleted
            });

            if (request.RaidTemplateId == 0)
            {
                // do stuff
            }

            return saveRaid;
        }
    }
}