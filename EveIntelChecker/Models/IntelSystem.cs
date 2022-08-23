namespace EveIntelChecker.Models
{
    public class IntelSystem
    {
        public int Jumps { get; set; } = 0;
        public int TriggerCounter { get; set; } = 0;
        public bool IsRed { get; set; } = false;
        public long SystemId { get; set; }
        public long SystemDomainId { get; set; }
        public long SystemConstellationId { get; set; }
        public string? SystemName { get; set; }
        public string? SystemDomainName { get; set; }
        public string? SystemConstellationName { get; set; }
        public List<long> ConnectedSytemsId { get; set; } = new List<long>();
    }
}
