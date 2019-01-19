namespace Core.Entities
{
    public class GuildRank
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int OrderBy { get; set; }
    }
}