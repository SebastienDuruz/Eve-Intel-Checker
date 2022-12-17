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
        public string System { get; set; }
        public string Region { get; set; }
        public string Shape { get; set; } = "box";
        public int BorderWidth { get; set; } = 1;
        public Colors Color { get; set; } = new Colors();
        public Font Font { get; set; } = new Font();
    }

    public class Colors
    {
        public string Background { get; set; } = "#27272fff";
        public string Border { get; set; } = "#E0E0E0";
    }

    public class Font 
    {
        public bool Multi { get; set; } = false;
    }
}
