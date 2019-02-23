using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/profession")]
    [ApiController]
    public class ProfessionController : ControllerBase
    {
        private readonly IProfessionRepository _professionRepository;
        public ProfessionController(IProfessionRepository ProfessionRepository)
        {
            _professionRepository = ProfessionRepository;
        }

        [HttpGet]
        public IEnumerable<Profession> Index()
        {
            return _professionRepository.LoadAll();
        }
    }
}