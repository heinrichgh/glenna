using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadRoleRestrictionTemplateRepository : IRepository<RaidEncounterSquadRoleRestrictionTemplate>
    {
        IEnumerable<RaidEncounterSquadRoleRestrictionTemplate> LoadByRole(int roleTempalteId);
        RaidEncounterSquadRoleRestrictionTemplate Save(RaidEncounterSquadRoleRestrictionTemplate raidEncounterSquadTemplate);
        RaidEncounterSquadRoleRestrictionTemplate Delete(int id);
    }
}