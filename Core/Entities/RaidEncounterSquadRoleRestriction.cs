namespace Core.Entities
{
    public class RaidEncounterSquadRoleRestriction
    {
        public int Id { get; set; }
        public int RaidEncounterSquadRoleId { get; set; }
        public int ProfessionId { get; set; }
        public int MinimumGuildRankId { get; set; }
    }
}