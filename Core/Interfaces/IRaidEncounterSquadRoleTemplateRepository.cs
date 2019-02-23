using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRoleTemplateRepository : IRepository<RaidEncounterSquadRoleTemplate>
    {
        RaidEncounterSquadRoleTemplate LoadSquadRole(int raidEncounterSquadTemplateId, int position);
        RaidEncounterSquadRoleTemplate Save(RaidEncounterSquadRoleTemplate raidEncounterSquadTemplate);
        RaidEncounterSquadRoleTemplate Delete(int id);
        IEnumerable<RaidEncounterSquadRoleTemplate> RemoveSquadRole(int raidEncounterSquadTemplateId);
    }
}