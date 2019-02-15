using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidTemplateRepository : IRepository<RaidTemplate>
    {
        RaidTemplate Load(string name);
        RaidTemplate Delete(int id);
    }
}