using Core.Entities.GuildWars;
using Core.Interfaces;

namespace Core.UseCases
{
    public class SignUpNewUser
    {
        private readonly IGuildWarsApi _guildWarsApi;

        public SignUpNewUser(IGuildWarsApi guildWarsApi)
        {
            _guildWarsApi = guildWarsApi;
        }

        public class SignUpNewUserRequest
        {
            public string ApiKey { get; set; }
        }
        
        public class SignUpNewUserResponse
        {
            public Account Account { get; set; }
            public bool Success { get; set; }
            public string Error { get; set; }
        }


        public SignUpNewUserResponse SignUp(SignUpNewUserRequest request)
        {
            
        }
    }
}