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
        
        public class CreateRaidTemplateResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public RaidTemplate SavedRaidTemplate { get; set; }
        }

        public async Task<CreateRaidTemplateResponse> CreateTemplate(RaidRequest request)
        {
            var guild = _guildRepository.Load(request.GuildId);
            if (guild != null)
            {
                if (_raidTemplateRepository.Load(request.TemplateName) != null)
                {
                    var saveRaidTemplate = _raidTemplateRepository.Save(new RaidTemplate
                    {
                        GuildId = request.GuildId,
                        Name = request.TemplateName
                    });
                    return new CreateRaidTemplateResponse { Response = "Created", Success = true, SavedRaidTemplate = saveRaidTemplate };
                }
                else
                {
                    return new CreateRaidTemplateResponse { Response = "Failed: " + request.TemplateName + " already exists.", Success = false};
                }
            }
            else
            {
                return new CreateRaidTemplateResponse { Response = "Failed: " + request.GuildId + " not found.", Success = false};
            }
        }
    }
}