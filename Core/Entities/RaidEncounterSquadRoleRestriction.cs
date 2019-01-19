namespace Core.Entities
{
    public class RaidEncounterSquadRoleRestriction
    {
        public uint Id { get; set; }
        public uint RaidEncounterSquadRoleId { get; set; }
        public uint ProfessionId { get; set; }
        public uint MinimumGuildRankId { get; set; }
    }
}