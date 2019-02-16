using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidRoleRepository : IRepository<RaidRole>
    {
        RaidRole Save(RaidRole raidRole);
        RaidRole Delete(int id);
    }
}