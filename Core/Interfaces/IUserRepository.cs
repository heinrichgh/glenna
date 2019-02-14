using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IUserRepository : IRepository<Member>
    {
        Member LoadByApiKey(string apiKey);
        Member Load(Guid gameGuid);
        Member Delete(int id);
        Member Delete(Guid gameGuid);
    }
}