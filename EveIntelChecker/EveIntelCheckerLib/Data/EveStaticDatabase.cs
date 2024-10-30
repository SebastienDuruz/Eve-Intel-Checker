using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EveIntelCheckerLib.Data
{
    /// <summary>
    /// Class EveStaticDatabase
    /// </summary>
    public class EveStaticDatabase
    {
        /// <summary>
        /// Padlock for thread safe Singleton operations
        /// </summary>
        private static readonly object _padLock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static EveStaticDatabase _instance = null;

        /// <summary>
        /// Path of the DB export files
        /// </summary>
        private static string FolderPath { get; set; } = string.Empty;

        /// <summary>
        /// List of SolarSystems
        /// </summary>
        public List<MapSolarSystem> SolarSystems { get; set; }

        /// <summary>
        /// List of JumpGates
        /// </summary>
        public List<MapSolarSystemJump> SolarSystemJumps { get; set; }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        private EveStaticDatabase()
        {
            FolderPath = Path.Combine(Directory.GetCurrentDirectory());

            // Resolve an issue with Blazor default folder (when Electron is not selected)
            //if (!Directory.Exists(FolderPath) || !File.Exists(Path.Combine(FolderPath, "mapRegion.json")))
            //    FolderPath = Path.Combine(this.GetType().Assembly.Location.Replace("EveIntelCheckerLib.dll", ""));

            SolarSystems = ReadSolarSystems();
            SolarSystemJumps = ReadSolarSystemJumps();
        }

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static EveStaticDatabase Instance
        {
            get
            {
                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = new EveStaticDatabase();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Read the solar systems export file
        /// </summary>
        /// <returns>List of solar systems</returns>
        public List<MapSolarSystem> ReadSolarSystems()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<MapSolarSystem>>(File.ReadAllText(Path.Combine(FolderPath, "mapSolarSystems.json")))!;
            }
            catch (Exception ex)
            {
                StaticData.Log(StaticData.LogLevel.Warning, ex.Message);
                return new List<MapSolarSystem>();
            }
        }

        /// <summary>
        /// Read the solar systems jumps export file
        /// </summary>
        /// <returns>List of solar systems</returns>
        public List<MapSolarSystemJump> ReadSolarSystemJumps()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<MapSolarSystemJump>>(File.ReadAllText(Path.Combine(FolderPath, "mapSolarSystemJumps.json")))!;
            }
            catch (Exception ex)
            {
                StaticData.Log(StaticData.LogLevel.Warning, ex.Message);
                return new List<MapSolarSystemJump>();
            }
        }

        /// <summary>
        /// Build the list of System to check
        /// </summary>
        /// <param name="root">The root system</param>
        /// <param name="systemDepth">The jumps to take before stopping the generation</param>
        /// <returns>The list with systems to check</returns>
        public List<IntelSystem> BuildSystemsList(MapSolarSystem? root, int systemDepth)
        {
            List<IntelSystem> intelSystems = [ConvertMapSytemToIntelSystem(root)];

            for (int i = 0; i < systemDepth; ++i)
                foreach (IntelSystem system in intelSystems.ToList())
                    foreach (long id in system.ConnectedSytemsId)
                        if (!intelSystems.Exists(x => x.SystemId == id))
                        {
                            IntelSystem current = ConvertMapSytemToIntelSystem(SolarSystems.Where(x => x.SolarSystemID == id).First());
                            current.Jumps = i + 1;
                            intelSystems.Add(current);
                        }

            return intelSystems;
        }

        /// <summary>
        /// Convert DB model to frontend object
        /// </summary>
        /// <param name="system">The DB object</param>
        /// <returns>The converted frontend object</returns>
        private IntelSystem ConvertMapSytemToIntelSystem(MapSolarSystem? system)
        {
            IntelSystem intelSystem = new IntelSystem();
            intelSystem.SystemId = system!.SolarSystemID;
            intelSystem.SystemName = system.SolarSystemName;

            foreach (MapSolarSystemJump connection in SolarSystemJumps.Where(x => x.FromSolarSystemID == intelSystem.SystemId))
                intelSystem.ConnectedSytemsId.Add(connection.ToSolarSystemID);

            return intelSystem;
        }
    }
}
