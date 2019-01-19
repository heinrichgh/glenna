namespace Core.Entities
{
    public class GuildRank
    {
        public uint Id { get; set; }
        public uint GuildId { get; set; }
        public string Name { get; set; }
        public uint OrderBy { get; set; }
    }
}