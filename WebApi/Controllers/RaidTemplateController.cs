using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raidtemplate")]
    [ApiController]
    public class RaidTemplateController : ControllerBase
    {
        private readonly IRepository<RaidTemplate> raidTemplateRepository;

        public RaidTemplateController(IRepository<RaidTemplate> raidTemplateRepository)
        {
            this.raidTemplateRepository = raidTemplateRepository;
        }

        [HttpGet]
        public IEnumerable<RaidTemplate> Index()
        {
            return raidTemplateRepository.LoadAll();
        }

        [HttpPut]
        public IEnumerable<RaidTemplate> Create(string Name, string test)
        {
            return null;
        }

        [HttpDelete]
        public IEnumerable<RaidTemplate> Remove(int RaidId)
        {
            return null;
        }
    }
}