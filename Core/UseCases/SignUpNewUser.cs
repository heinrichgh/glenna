using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.GuildWars;
using Core.Interfaces;

namespace Core.UseCases
{
    public class SignUpNewUser
    {
        private readonly IGuildWarsApi _guildWarsApi;
        private readonly IGuildWarsAccountRepository _guildWarsAccountRepository;

        public SignUpNewUser(IGuildWarsApi guildWarsApi, IGuildWarsAccountRepository guildWarsAccountRepository)
        {
            _guildWarsApi = guildWarsApi;
            _guildWarsAccountRepository = guildWarsAccountRepository;
        }

        public class SignUpNewUserRequest
        {
            public string ApiKey { get; set; }
        }
        
        public class SignUpNewUserResponse
        {
            public GuildwarsAccount Account { get; set; }
            public bool Success { get; set; }
            public string Error { get; set; }
        }


        public async Task<SignUpNewUserResponse> SignUp(SignUpNewUserRequest request)
        {
//            var guildWarsAccount = _guildWarsAccountRepository.LoadByApiKey(request.ApiKey);

            var account = await _guildWarsApi.Fetch(request.ApiKey);

            var savedAccount =_guildWarsAccountRepository.Save(new GuildwarsAccount
            {
                ApiKey = request.ApiKey,
                GameGuid = account.Id.ToString(),
                IsCommander = account.Commander
            });
            
            return new SignUpNewUserResponse
            {
                Success = true,
                Account = savedAccount
            };
        }
    }
}