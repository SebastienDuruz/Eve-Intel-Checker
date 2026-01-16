using EveIntelCheckerLib.Models;
using System.Collections.Generic;

namespace EveIntelCheckerLib.Services
{
    public interface IIntelMessageProcessor
    {
        IntelMessageResult Process(string message, IList<IntelSystem> systems, UserSettings settings);
    }
}
