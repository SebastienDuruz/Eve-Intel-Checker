using EveIntelCheckerLib.Models.Map;
using System;

namespace EveIntelCheckerLib.Services
{
    public sealed class MapData
    {
        public static readonly MapData Empty = new MapData(Array.Empty<MapNode>(), Array.Empty<MapLink>());

        public MapData(MapNode[] nodes, MapLink[] links)
        {
            Nodes = nodes;
            Links = links;
        }

        public MapNode[] Nodes { get; }
        public MapLink[] Links { get; }
    }
}
