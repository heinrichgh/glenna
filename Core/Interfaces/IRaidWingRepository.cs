using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidWingRepository
    {
        IEnumerable<RaidWing> LoadAll();
        RaidWing Load(uint id);
    }
}