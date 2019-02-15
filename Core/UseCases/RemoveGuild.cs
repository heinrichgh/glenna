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
        private readonly IRaidTemplateRepository _raidTemplateRepository;

        public RemoveGuild(IGuildRepository guildRepository, IRaidTemplateRepository raidTemplateRepository)
        {
            _guildRepository = guildRepository;
            _raidTemplateRepository = raidTemplateRepository;
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
                        response.RemovedTemplates.Add(_raidTemplateRepository.Delete(raidTemplate.Id)); 
                    }
                }
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