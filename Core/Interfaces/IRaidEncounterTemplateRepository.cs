using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterTemplateRepository : IRepository<RaidEncounterTemplate>
    {
        RaidEncounterTemplate Load(int raidTemplateId, int raidBossId);
        RaidEncounterTemplate Save(RaidEncounterTemplate raidEncounterTemplate);
        RaidEncounterTemplate Delete(int id);
    }
}