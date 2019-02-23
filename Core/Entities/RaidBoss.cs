namespace Core.Entities
{
    public class RaidBoss
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int RaidWingId { get; set; }
        public bool HasCm { get; set; }
        public string ShortName { get; set; }
        public string Emoji { get; set; }

    }
}