using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGuildRepository : IRepository<Guild>
    {
        Guild Load(Guid gameGuid);
        Guild Delete(int id);
        Guild Delete(Guid gameGuid);
    }
}