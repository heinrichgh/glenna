using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/guild/rank")]
    [ApiController]
    public class GuildRankController : ControllerBase
    {
        private readonly IGuildRankRepository _guildRankRepository;

        public GuildRankController(IGuildRankRepository guildRankRepository)
        {
            _guildRankRepository = guildRankRepository;
        }

        [HttpGet]
        public IEnumerable<GuildRank> Index()
        {
            return _guildRankRepository.LoadAll();
        }

        [Route("user/{memberId}")]
        [HttpGet]
        public GuildRank GetUserRank(int memberId)
        {
            return _guildRankRepository.LoadMember(memberId);
        }

        [Route("guild/{guildId}")]
        [HttpGet]
        public IEnumerable<GuildRank> GetGuildRank(int guildId)
        {
            return _guildRankRepository.LoadGuild(guildId);
        }
    }
}