namespace Core.Entities
{
    public class RaidEncounterSquad
    {
        public int Id { get; set; }
        public int RaidEncounterId { get; set; }
        public int Position { get; set; }
        public int GuildMemberId { get; set; }
    }
}