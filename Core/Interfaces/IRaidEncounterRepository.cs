using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterRepository : IRepository<RaidEncounter>
    {
        RaidEncounter Load(int raidId, int raidBossId);
        IEnumerable<RaidEncounter> LoadByRaidId(int raidId);
        RaidEncounter Save(RaidEncounter raidEncounter);
        RaidEncounter Delete(int id);
        void RemoveRaidEncounter(int raidId);
    }
}