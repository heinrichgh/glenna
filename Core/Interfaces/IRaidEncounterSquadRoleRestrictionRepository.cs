using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRoleRestrictionRepository : IRepository<RaidEncounterSquadRoleRestriction>
    {
        IEnumerable<RaidEncounterSquadRoleRestriction> LoadRoleRestrictions(int squadRoleId);
        RaidEncounterSquadRoleRestriction Save(RaidEncounterSquadRoleRestriction raidEncounterSquadRoleRestriction);
        RaidEncounterSquadRoleRestriction Delete(int id);
    }
}