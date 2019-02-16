using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRoleTemplateRepository : IRepository<RaidEncounterSquadRoleTemplate>
    {
        IEnumerable<RaidEncounterSquadRoleTemplate> LoadSquadRole(int raidEncounterSquadTemplateId);
        RaidEncounterSquadRoleTemplate Save(RaidEncounterSquadRoleTemplate raidEncounterSquadTemplate);
        RaidEncounterSquadRoleTemplate Delete(int id);
        IEnumerable<RaidEncounterSquadRoleTemplate> RemoveSquadRole(int raidEncounterSquadTemplateId);
    }
}