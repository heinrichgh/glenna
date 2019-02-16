using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.UseCases
{
    public class RemoveDiscordAccount
    {
        private readonly IDiscordAccountRepository _discordAccountRepository;
        public RemoveDiscordAccount(IDiscordAccountRepository discordAccountRepository)
        {
            _discordAccountRepository = discordAccountRepository;
        }

        public class DiscordAccountRequest
        {
            public int DiscordAccountId { get; set; }
        }

        public class RemoveDiscordAccountResponse
        {
            public string Response { get; set; }
            public bool Success { get; set; }
            public DiscordAccount RemovedDiscordAccount { get; set; }
        }

        public async Task<RemoveDiscordAccountResponse> Remove(DiscordAccountRequest request)
        {
            RemoveDiscordAccountResponse response = new RemoveDiscordAccountResponse();
            if (_discordAccountRepository.Load(request.DiscordAccountId) == null)
            {
                response.Response = $"Not found ID: {request.DiscordAccountId}";
                response.Success = false;
            }
            else
            {
                response.RemovedDiscordAccount = _discordAccountRepository.Delete(request.DiscordAccountId);
                response.Response = $"Removed ID: {request.DiscordAccountId}";
                response.Success = true;
            }
            return response;
        }
    }
}