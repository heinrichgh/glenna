using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidRepository : IRepository<Raid>
    {
        Raid Save(Raid raid);
        Raid Delete(int id);
    }
}