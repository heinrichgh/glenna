using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class CreateRaidTemplate
    {
        private readonly IRaidTemplateRepository _raidTemplateRepository;
        private readonly IGuildRepository _guildRepository;

        public CreateRaidTemplate(IGuildRepository guildRepository, IRaidTemplateRepository raidTemplateRepository)
        {
            _guildRepository = guildRepository;
            _raidTemplateRepository = raidTemplateRepository;
        }

        public class RaidRequest
        {
            public int GuildId { get; set; }
            public string TemplateName { get; set; }
        }
        
        public async Task<RaidTemplate> CreateTemplate(RaidRequest request)
        {
            var guild = _guildRepository.Load(request.GuildId);
            if (guild != null)
            {                
                var saveRaidTemplate = _raidTemplateRepository.Save(new RaidTemplate
                {
                    GuildId = request.GuildId,
                    Name = request.TemplateName
                });

                return saveRaidTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}