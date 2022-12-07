/// Author : Sébastien Duruz
/// Date : 05.12.2022
namespace EveIntelCheckerLib.Models.Map
{
    /// <summary>
    /// Class MapNode
    /// </summary>
    public class MapNode
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public string Shape { get; set; } = "box";
        public Colors Color { get; set; } = new Colors();
    }

    public class Colors
    {
        public string Background { get; set; } = "#27272fff";
        public string Border { get; set; } = "#E0E0E0";
    }
}
