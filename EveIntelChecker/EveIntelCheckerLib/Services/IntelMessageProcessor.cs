using EveIntelCheckerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EveIntelCheckerLib.Services
{
    public sealed class IntelMessageProcessor : IIntelMessageProcessor
    {
        public IntelMessageResult Process(string message, IList<IntelSystem> systems, UserSettings settings)
        {
            IntelMessageResult result = new IntelMessageResult();
            if (string.IsNullOrWhiteSpace(message) || systems.Count == 0)
                return result;

            foreach (IntelSystem intelSystem in systems)
            {
                if (!message.Contains(intelSystem.SystemName))
                    continue;

                if (settings.ClearResetCounter
                    && (settings.ExcludeFilters?.Any(filter => message.Contains(filter)) ?? false))
                {
                    intelSystem.IsRed = false;
                    intelSystem.TriggerCounter = 0;
                }
                else
                {
                    intelSystem.IsRed = true;

                    if (intelSystem.Jumps < settings.IgnoreNotification)
                    {
                        bool isDanger = intelSystem.Jumps <= settings.DangerNotification;
                        result.Notifications.Add(new IntelNotification
                        {
                            IsDanger = isDanger,
                            SystemName = intelSystem.SystemName
                        });
                    }

                    ++intelSystem.TriggerCounter;
                }

                result.NewRedSystemName = intelSystem.SystemName;
            }

            if (!string.IsNullOrWhiteSpace(result.NewRedSystemName))
            {
                foreach (IntelSystem intelSystem in systems.Where(x => x.SystemName != result.NewRedSystemName))
                    intelSystem.IsRed = false;
            }

            result.HasChanges = !string.IsNullOrWhiteSpace(result.NewRedSystemName);
            return result;
        }
    }
}
