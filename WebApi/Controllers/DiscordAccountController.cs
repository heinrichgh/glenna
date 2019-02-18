using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using static Core.UseCases.AddDiscordAccount;
using static Core.UseCases.RemoveDiscordAccount;

namespace WebApi.Controllers
{
    [Route("api/discord/account")]
    [ApiController]
    public class DiscordAccountController : ControllerBase
    {
        private readonly IDiscordAccountRepository _discordAccountRepository; 
        private readonly AddDiscordAccount _addDiscordAccount;
        private readonly RemoveDiscordAccount _removeDiscordAccount;
        public DiscordAccountController(IDiscordAccountRepository discordAccountRepository, AddDiscordAccount addDiscordAccount, RemoveDiscordAccount removeDiscordAccount)
        {
            _discordAccountRepository = discordAccountRepository;
            _addDiscordAccount = addDiscordAccount;
            _removeDiscordAccount = removeDiscordAccount;
        }

        [HttpGet]
        public IEnumerable<DiscordAccount> Index()
        {
            return _discordAccountRepository.LoadAll();
        }

        [Route("user/{userId}")]
        [HttpGet]
        public DiscordAccount GetUser(int userId)
        {
            return _discordAccountRepository.LoadUser(userId);
        }

        [HttpPut]
        public async Task<AddDiscordAccountResponse> Create(string discordAccountIdentity, int userId, string status)
        {
            Regex.Replace(discordAccountIdentity, "[!]", string.Empty);
            return await _addDiscordAccount.Add(new AddDiscordAccount.DiscordAccountRequest
            {
                DiscordIdentity = discordAccountIdentity,
                MemberId = userId,
                Status = status
            });
        }

        [HttpDelete]
        public async Task<RemoveDiscordAccountResponse> Remove(int discordAccountId)
        {
            return await _removeDiscordAccount.Remove(new RemoveDiscordAccount.DiscordAccountRequest
            {
                DiscordAccountId = discordAccountId
            });
        }
    }
}