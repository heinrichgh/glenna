using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class AddDiscordAccount
    {
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IMemberDiscordAccountRepository _memberDiscordAccountRepository;

        public AddDiscordAccount(IDiscordAccountRepository discordAccountRepository, IMemberDiscordAccountRepository memberDiscordAccountRepository)
        {
            _discordAccountRepository = discordAccountRepository;
            _memberDiscordAccountRepository = memberDiscordAccountRepository;
        }

        public class DiscordAccountRequest
        {
            public string DiscordIdentity { get; set; }
            public int MemberId { get; set; }
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
            if (_discordAccountRepository.Load(request.DiscordIdentity) == null)
            {
                var savedDiscordAccount = _discordAccountRepository.Save(new DiscordAccount
                {
                    DiscordIdentity = request.DiscordIdentity,
                    Status = request.Status,
                    CreatedAt = DateTime.Now
                });
                var savedMemberDiscordAccount = _memberDiscordAccountRepository.Save(new MemberDiscordAccount
                {
                    DiscordAccountId = savedDiscordAccount.Id,
                    MemberId = request.MemberId
                });
                response.Response = "Created";
                response.Success = true;
                response.SavedDiscordAccount = savedDiscordAccount;
            }
            else
            {
                response.Response = "Account Exists";
                response.Success = false;
            }
           

            return response;
        }
    }
}