using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IMemberDiscordAccountRepository : IRepository<MemberDiscordAccount>
    {
        MemberDiscordAccount Save(MemberDiscordAccount memberDiscordAccount);
        MemberDiscordAccount Delete(int id);
        void RemoveMember(int memberId);
    }
}