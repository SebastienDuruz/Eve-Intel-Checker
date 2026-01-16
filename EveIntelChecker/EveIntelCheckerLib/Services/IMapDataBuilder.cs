using EveIntelCheckerLib.Models;
using System.Collections.Generic;

namespace EveIntelCheckerLib.Services
{
    public interface IMapDataBuilder
    {
        MapData Build(IReadOnlyList<IntelSystem> systems);
    }
}
