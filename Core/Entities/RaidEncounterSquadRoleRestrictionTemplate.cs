namespace Core.Entities
{
    public class RaidEncounterSquadRoleRestrictionTemplate
    {
        public uint Id { get; set; }
        public uint RaidEncounterSquadRoleTemplateId { get; set; }
        public uint ProfessionId { get; set; }
        public uint MinimumGuildRankId { get; set; }
    }
}