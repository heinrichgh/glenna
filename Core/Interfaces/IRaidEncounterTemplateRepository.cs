using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterTemplateRepository : IRepository<RaidEncounterTemplate>
    {
        IEnumerable<RaidEncounterTemplate> LoadByTemplate(int raidTemplateId);
        RaidEncounterTemplate Load(int raidTemplateId, int raidBossId);
        RaidEncounterTemplate Save(RaidEncounterTemplate raidEncounterTemplate);
        RaidEncounterTemplate Delete(int id);
    }
}