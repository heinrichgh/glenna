namespace Core.Entities
{
    public class RaidEncounterSquad
    {
        public uint Id { get; set; }
        public uint RaidEncounterId { get; set; }
        public uint Position { get; set; }
        public uint GuildMemberId { get; set; }
    }
}