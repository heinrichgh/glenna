using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidRepository : IRepository<Raid>
    {
        IEnumerable<Raid> LoadGuildRaids(int guildId);
        Raid Save(Raid raid);
        Raid Delete(int id);
    }
}