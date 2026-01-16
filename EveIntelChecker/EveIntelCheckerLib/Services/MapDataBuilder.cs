using EveIntelCheckerLib.Data;
using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Map;
using System;
using System.Collections.Generic;

namespace EveIntelCheckerLib.Services
{
    public sealed class MapDataBuilder : IMapDataBuilder
    {
        public MapData Build(IReadOnlyList<IntelSystem> systems)
        {
            if (systems == null || systems.Count == 0)
                return MapData.Empty;

            MapNode[] mapNodes = new MapNode[systems.Count];
            List<MapLink> mapLinks = new List<MapLink>();
            Dictionary<long, long> nodeIdsBySystemId = new Dictionary<long, long>(systems.Count);

            for (int i = 0; i < systems.Count; ++i)
            {
                mapNodes[i] = new MapNode
                {
                    Color =
                    {
                        Background = "#1c1c1cff"
                    }
                };

                if (systems[i].IsRed)
                    mapNodes[i].Color.Background = "#ff3f5fff";
                else if (systems[i].TriggerCounter > 0)
                    mapNodes[i].Color.Background = "#ff9800ff";

                if (systems[i].Jumps == 0)
                {
                    mapNodes[i].Shape = "ellipse";
                    mapNodes[i].BorderWidth = 2;
                }

                mapNodes[i].Font.Multi = true;
                mapNodes[i].Label = $"{systems[i].SystemName}\n<code>J:{systems[i].Jumps} T:{systems[i].TriggerCounter}</code>";
                mapNodes[i].Id = i + 1;
                mapNodes[i].System = systems[i].SystemName;
                nodeIdsBySystemId[systems[i].SystemId] = mapNodes[i].Id;
            }

            foreach (IntelSystem system in systems)
            {
                foreach (long link in system.ConnectedSytemsId)
                {
                    MapLink systemLink = new MapLink();

                    try
                    {
                        if (!nodeIdsBySystemId.TryGetValue(system.SystemId, out long fromId))
                            continue;
                        if (!nodeIdsBySystemId.TryGetValue(link, out long toId))
                            continue;

                        systemLink.From = fromId;
                        systemLink.To = toId;
                        if (!mapLinks.Exists(x => x.From == systemLink.From && x.To == systemLink.To) &&
                            !mapLinks.Exists(x => x.From == systemLink.To && x.To == systemLink.From))
                            mapLinks.Add(systemLink);
                    }
                    catch (Exception ex)
                    {
                        LogsWriter.Instance.Log(StaticData.LogLevel.Error, ex.Message);
                    }
                }
            }

            return new MapData(mapNodes, mapLinks.ToArray());
        }
    }
}
