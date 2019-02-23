using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Discord;

namespace Core.Interfaces
{
    public interface IDiscordWebhook
    {
        Task<Webhook> FetchAccount(string webhook);
        void PostMessage(string webhook, string message);
    }
}