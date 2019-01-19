namespace Core.Entities
{
    public class RaidBoss
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Order { get; set; }
        public uint RaidWingId { get; set; }
        public bool HasCm { get; set; }
    }
}