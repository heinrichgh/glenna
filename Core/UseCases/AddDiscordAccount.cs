using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddDiscordAccount
    {
        private readonly IDiscordAccountRepository _discordAccountRepository;

        public AddDiscordAccount(IDiscordAccountRepository discordAccountRepository)
        {
            _discordAccountRepository = discordAccountRepository;
        }

        public class DiscordAccountRequest
        {
            public string DiscordIdentity { get; set; }
            public string Status { get; set; }
        }
        
        public class AddDiscordAccountResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public DiscordAccount SavedDiscordAccount { get; set; }
        }

        public async Task<AddDiscordAccountResponse> Add(DiscordAccountRequest request)
        {
            AddDiscordAccountResponse response = new AddDiscordAccountResponse();
            var savedDiscordAccount = _discordAccountRepository.Save(new DiscordAccount
            {
                DiscordIdentity = request.DiscordIdentity,
                Status = request.Status,
                CreatedAt = DateTime.Now
            });
            response.Response = "Created";
            response.Success = true;
            response.SavedDiscordAccount = savedDiscordAccount;

            return response;
        }
    }
}