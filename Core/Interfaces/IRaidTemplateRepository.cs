using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidTemplateRepository : IRepository<RaidTemplate>
    {
        RaidTemplate Delete(int id);
    }
}