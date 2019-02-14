using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveUser
    {
        private readonly IUserRepository _userRepository;

        public RemoveUser(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public class UserRequest
        {
            public Guid GameGuid { get; set; }
            public int Id { get; set; }
        }

        public Member Remove(UserRequest request)
        {
            if (request.GameGuid == null)
            {
                return _userRepository.Delete(request.Id);
            }
            
            return _userRepository.Delete(request.GameGuid);
            
            
        }
    }
}