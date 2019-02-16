using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveGuild
    {
        private readonly IGuildRepository _guildRepository;
        private readonly IGuildRankRepository _guildRankRepository;
        private readonly IGuildMemberRepository _guildMemberRepository;
        private readonly IRaidRepository _raidRepository;
        private readonly IRaidTemplateRepository _raidTemplateRepository;
        private readonly RemoveRaidTemplate _removeRaidTemplate;
        private readonly RemoveRaid _removeRaid;

        public RemoveGuild(IGuildRepository guildRepository, IRaidRepository raidRepository, RemoveRaid removeRaid, RemoveRaidTemplate removeRaidTemplate, IRaidTemplateRepository raidTemplateRepository, IGuildRankRepository guildRankRepository, IGuildMemberRepository guildMemberRepository)
        {
            _guildRepository = guildRepository;
            _guildRankRepository = guildRankRepository;
            _guildMemberRepository = guildMemberRepository;
            _raidRepository = raidRepository;
            _raidTemplateRepository = raidTemplateRepository;
            _removeRaidTemplate = removeRaidTemplate;
            _removeRaid = removeRaid;
        }

        public class GuildRequest
        {
            public Guid GuildGuid { get; set; }
        }

        public class RemoveGuildResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Guild RemovedGuild { get; set; }
            public List<RaidTemplate> RemovedTemplates { get; set; }
            public List<GuildRank> RemovedRanks { get; set; }
        }

        public RemoveGuildResponse Remove(GuildRequest request)
        {
            if (_guildRepository.Load(request.GuildGuid) != null)
            {
                RemoveGuildResponse response = new RemoveGuildResponse();
                foreach (RaidTemplate raidTemplate in _raidTemplateRepository.LoadAll())
                {
                    if (raidTemplate.GuildId == _guildRepository.Load(request.GuildGuid).Id)
                    {
                        var removedRaidTemplate = _removeRaidTemplate.Remove(new RemoveRaidTemplate.RaidRequest {
                            RaidTemplateId = raidTemplate.Id
                        }); 
                    }
                }
                foreach (Raid raid in _raidRepository.LoadAll())
                {
                    if (raid.GuildId == _guildRepository.Load(request.GuildGuid).Id)
                    {
                        var removedRaid = _removeRaid.Remove(new RemoveRaid.RaidRequest
                        {
                            RaidId = raid.Id
                        });
                    }
                }
                _guildMemberRepository.RemoveGuild(_guildRepository.Load(request.GuildGuid).Id);
                _guildRankRepository.RemoveGuild(_guildRepository.Load(request.GuildGuid).Id);
                response.Response = "Removed";
                response.Success = true;
                response.RemovedGuild = _guildRepository.Delete(request.GuildGuid);
                return response;
            }
            else
            {
                return new RemoveGuildResponse { Response = "Failed: " + request.GuildGuid + " not found.", Success = false };
            }
        }
    }
}