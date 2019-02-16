using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRepository : IRepository<RaidEncounterSquad>
    {
        IEnumerable<RaidEncounterSquad> LoadSquad(int raidEncounterId);
        RaidEncounterSquad Save(RaidEncounterSquad raidEncounterSquad);
        RaidEncounterSquad Delete(int id);
        IEnumerable<RaidEncounterSquad> RemoveSquad(int raidEncounterId);
    }
}