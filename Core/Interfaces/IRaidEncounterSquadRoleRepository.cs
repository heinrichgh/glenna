using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRoleRepository : IRepository<RaidEncounterSquadRole>
    {
        IEnumerable<RaidEncounterSquadRole> LoadSquadRole(int raidEncounterSquadId);
        RaidEncounterSquadRole Save(RaidEncounterSquadRole raidEncounterSquad);
        RaidEncounterSquadRole Delete(int id);
        IEnumerable<RaidEncounterSquadRole> RemoveSquadRole(int raidEncounterSquadId);
    }
}