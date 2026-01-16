using System.Collections.Generic;

namespace EveIntelCheckerLib.Services
{
    public sealed class IntelNotification
    {
        public bool IsDanger { get; init; }
        public string SystemName { get; init; } = string.Empty;
    }

    public sealed class IntelMessageResult
    {
        public bool HasChanges { get; set; }
        public string? NewRedSystemName { get; set; }
        public List<IntelNotification> Notifications { get; } = new List<IntelNotification>();
    }
}
