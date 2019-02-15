using System;
using System.Threading.Tasks;
using Core.Entities;
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
        }

        public class RemoveUserResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public Member RemovedUser { get; set; }
        }

        public RemoveUserResponse Remove(UserRequest request)
        {
            if (_userRepository.Load(request.GameGuid) != null)
            {
                return new RemoveUserResponse { Response = "Removed", Success = true, RemovedUser = _userRepository.Delete(request.GameGuid) };
            }
            else
            {
                return new RemoveUserResponse { Response = "Failed: " + request.GameGuid + " not found", Success = false };
            }
        }
    }
}