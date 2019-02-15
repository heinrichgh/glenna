using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.CreateRaidTemplate;
using static Core.UseCases.RemoveRaidTemplate;

namespace WebApi.Controllers
{
    [Route("api/raidtemplate")]
    [ApiController]
    public class RaidTemplateController : ControllerBase
    {
        private readonly IRaidTemplateRepository _raidTemplateRepository;
        private readonly IGuildRepository _guildRepository;
        private readonly CreateRaidTemplate _createRaidTemplate;
        private readonly RemoveRaidTemplate _removeRaidTemplate;

        public RaidTemplateController(IRaidTemplateRepository raidTemplateRepository, IGuildRepository guildRepository, CreateRaidTemplate createRaidTemplate, RemoveRaidTemplate removeRaidTemplate)
        {
            _raidTemplateRepository = raidTemplateRepository;
            _guildRepository = guildRepository;
            _createRaidTemplate = createRaidTemplate;
            _removeRaidTemplate = removeRaidTemplate;
        }

        [HttpGet]
        public IEnumerable<RaidTemplate> Index()
        {
            return _raidTemplateRepository.LoadAll();
        }

        [HttpPut]
        public async Task<CreateRaidTemplateResponse> Create(string name, int guildId)
        {
            if (_guildRepository.Load(guildId) == null)
                {
                    guildId = 0;
                }

            return await _createRaidTemplate.CreateTemplate(new CreateRaidTemplate.RaidRequest { 
                GuildId = guildId,
                TemplateName = name});
        }

        [HttpDelete]
        public async Task<RemoveRaidTemplateResponse> Remove(int raidId)
        {
            return await _removeRaidTemplate.Remove(new RemoveRaidTemplate.RaidRequest {
                RaidTemplateId = raidId
            });
        }
    }
}