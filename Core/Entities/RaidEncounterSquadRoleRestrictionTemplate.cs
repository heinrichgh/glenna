namespace Core.Entities
{
    public class RaidEncounterSquadRoleRestrictionTemplate
    {
        public int Id { get; set; }
        public int RaidEncounterSquadRoleTemplateId { get; set; }
        public int ProfessionId { get; set; }
        public int MinimumGuildRankId { get; set; }
    }
}