using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidRepository : IRepository<Raid>
    {
        Raid Delete(int id);
    }
}