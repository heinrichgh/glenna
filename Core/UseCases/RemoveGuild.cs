using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveGuild
    {
        private readonly IGuildRepository _guildRepository;

        public RemoveGuild(IGuildRepository userRepository)
        {
            _guildRepository = userRepository;
        }

        public class GuildRequest
        {
            public Guid GuildGuid { get; set; }
            public int Id { get; set; }
        }
        
        public class SignUpUserResponse
        {
            public Guild Guild { get; set; }
        }


        public Guild Remove(GuildRequest request)
        {
            if (request.GuildGuid == null)
            {
                return _guildRepository.Delete(request.Id);
            }
            
            return _guildRepository.Delete(request.GuildGuid);
        }
    }
}