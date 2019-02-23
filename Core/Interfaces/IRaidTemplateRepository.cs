using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidTemplateRepository : IRepository<RaidTemplate>
    {
        RaidTemplate Load(string name);
        IEnumerable<RaidTemplate> LoadGuildTemplates(int guildId);
        RaidTemplate Save(RaidTemplate raidTemplate);
        RaidTemplate Delete(int id);
    }
}