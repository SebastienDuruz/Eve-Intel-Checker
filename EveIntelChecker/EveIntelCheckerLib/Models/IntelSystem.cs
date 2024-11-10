using System.Collections.Generic;

namespace EveIntelCheckerLib.Models
{
    /// <summary>
    /// Class IntelSystem
    /// </summary>
    public class IntelSystem
    {
        /// <summary>
        /// Nb of jumps from the root system
        /// </summary>
        public int Jumps { get; set; } = 0;

        /// <summary>
        /// Nb of triggers detected for the system
        /// </summary>
        public int TriggerCounter { get; set; } = 0;

        /// <summary>
        /// Does the last trigger correspond to the system ?
        /// </summary>
        public bool IsRed { get; set; } = false;

        /// <summary>
        /// Id of the system
        /// </summary>
        public long SystemId { get; set; }

        /// <summary>
        /// Name of the system
        /// </summary>
        public string SystemName { get; set; } = string.Empty;

        /// <summary>
        /// List of system ID directly connected with this system
        /// </summary>
        public List<long> ConnectedSytemsId { get; set; } = new List<long>();
    }
}
