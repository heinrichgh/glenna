using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidTemplateRepository : IRepository<RaidTemplate>
    {
        RaidTemplate Load(string name);
        RaidTemplate Save(RaidTemplate raidTemplate);
        RaidTemplate Delete(int id);
    }
}