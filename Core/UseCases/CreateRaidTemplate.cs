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
            CreateRaidTemplateResponse response = new CreateRaidTemplateResponse();
            if (request.GuildId == 0 || request.TemplateName == null)
            {
                response.Response = "Invalid Guild or Name";
                response.Success = false;
            }
            else
            {
                var guild = _guildRepository.Load(request.GuildId);
                if (guild == null)
                {
                    response.Response = "Incorrect Guild ID";
                    response.Success = false;
                }
                else
                {
                    var existingTemplate = _raidTemplateRepository.Load(request.TemplateName);
                    if (existingTemplate == null)
                    {
                        var saveRaidTemplate = _raidTemplateRepository.Save(new RaidTemplate
                        {
                            GuildId = request.GuildId,
                            Name = request.TemplateName
                        });
                        response.Response = "Template Created";
                        response.Success = true;
                        response.SavedRaidTemplate = saveRaidTemplate;
                        
                    }
                    else
                    {
                        response.Response = "Template Name already exists";
                        response.Success = false;
                    }
                }
            }
            return response;
        }
    }
}