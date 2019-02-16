using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRaidEncounterSquadTemplateRepository : IRepository<RaidEncounterSquadTemplate>
    {
        IEnumerable<RaidEncounterSquadTemplate> LoadSquad(int raidEncounterTemplateId);
        RaidEncounterSquadTemplate Save(RaidEncounterSquadTemplate raidEncounterSquadTemplate);
        RaidEncounterSquadTemplate Delete(int id);
        IEnumerable<RaidEncounterSquadTemplate> RemoveSquad(int raidEncounterTemplateId);
    }
}