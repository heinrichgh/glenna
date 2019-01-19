using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/raid")]
    [ApiController]
    public class RaidController : ControllerBase
    {
        private readonly IRepository<Raid> raidRepository;

        public RaidController(IRepository<Raid> raidRepository)
        {
            this.raidRepository = raidRepository;
        }

        [HttpGet]
        public IEnumerable<Raid> Index()
        {
            return raidRepository.LoadAll();
        }

        [HttpPut]
        public IEnumerable<Raid> Create(string DisdcordIdentity, int TemplateId)
        {
            return null;
        }

        [HttpDelete]
        public IEnumerable<Raid> Remove(int RaidId)
        {
            return null;
        }
    }
}