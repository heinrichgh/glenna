using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterRepository : IRepository<RaidEncounter>
    {
        RaidEncounter Load(int raidId, int raidBossId);
        RaidEncounter Save(RaidEncounter raidEncounter);
        RaidEncounter Delete(int id);
    }
}