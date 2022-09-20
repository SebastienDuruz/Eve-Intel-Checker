/// Author : Sébastien Duruz
/// Date : 23.08.2022

using EveIntelCheckerLib.Models;
using EveIntelCheckerLib.Models.Database;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
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
        /// Path of the DB file
        /// </summary>
        private static string DbPath { get; } = Path.Combine(Environment.ProcessPath.Replace("EveIntelChecker.exe", ""), "eve.db");

        /// <summary>
        /// Connection object for SQLite Database
        /// </summary>
        private SQLiteConnection SqliteConnection { get; set; }

        /// <summary>
        /// List of SolarSystems
        /// </summary>
        public List<MapSolarSystem> SolarSystems { get; set; }

        /// <summary>
        /// List of JumpGates
        /// </summary>
        public List<MapSolarSystemJump> SolarSystemJumps { get; set; }

        /// <summary>
        /// List of Regions
        /// </summary>
        public List<MapRegion> Regions { get; set; }

        /// <summary>
        /// List of Constellations
        /// </summary>
        public List<MapConstellation> Constellations { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EveStaticDatabase()
        {
            CreateConnection();

            SolarSystems = this.ReadSolarSystems();
            SolarSystemJumps = this.ReadSolarSystemsJumps();
            Regions = this.ReadRegions();
            Constellations = this.ReadConstellations();
        }

        /// <summary>
        /// Establish the connection with the SQLite Database
        /// </summary>
        /// <returns>An object that contains the connection to Sqlite DB</returns>
        private void CreateConnection()
        {
            SqliteConnection = new SQLiteConnection(DbPath);
        }

        /// <summary>
        /// Read the SolarSystems from Sqlite DB
        /// </summary>
        /// <returns>List of SolarSystems</returns>
        private List<MapSolarSystem> ReadSolarSystems()
        {
            List<MapSolarSystem> solarSystems = new List<MapSolarSystem>();
            string query = @"SELECT regionID, constellationID, solarSystemID, solarSystemName FROM mapSolarSystems";

            return SqliteConnection.Query<MapSolarSystem>(query);
        }

        /// <summary>
        /// Read the JumpGates from Sqlite DB
        /// </summary>
        /// <returns>List of JumpGates</returns>
        private List<MapSolarSystemJump> ReadSolarSystemsJumps()
        {
            List<MapSolarSystemJump> solarSystemJumps = new List<MapSolarSystemJump>();
            string query = @"SELECT fromSolarSystemID, toSolarSystemID FROM mapSolarSystemJumps";

            return SqliteConnection.Query<MapSolarSystemJump>(query);
        }

        /// <summary>
        /// Read the Regions from Sqlite DB
        /// </summary>
        /// <returns>List of Regions</returns>
        private List<MapRegion> ReadRegions()
        {
            List<MapRegion> regions = new List<MapRegion>();
            string query = @"SELECT regionID, regionName FROM mapRegions";

            return SqliteConnection.Query<MapRegion>(query);
        }

        /// <summary>
        /// Read the Constellations from Sqlite DB
        /// </summary>
        /// <returns>List of Constellations</returns>
        private List<MapConstellation> ReadConstellations()
        {
            List<MapConstellation> constellations = new List<MapConstellation>();
            string query = @"SELECT regionID, constellationID, constellationName FROM mapConstellations";

            return SqliteConnection.Query<MapConstellation>(query);
        }

        /// <summary>
        /// Build the list of System to check
        /// </summary>
        /// <param name="root">The root system</param>
        /// <returns>The list with systems to check</returns>
        public List<IntelSystem> BuildSystemsList(MapSolarSystem root)
        {
            List<IntelSystem> intelSystems = new List<IntelSystem>();
            intelSystems.Add(ConvertMapSytemToIntelSystem(root));

            // Depth of 5 jumps by default
            for (int i = 0; i < 5; ++i)
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
        private IntelSystem ConvertMapSytemToIntelSystem(MapSolarSystem system)
        {
            IntelSystem intelSystem = new IntelSystem();
            intelSystem.SystemId = system.SolarSystemID;
            intelSystem.SystemName = system.SolarSystemName;
            intelSystem.SystemDomainId = system.RegionID;
            intelSystem.SystemConstellationId = system.ConstellationID;
            intelSystem.SystemDomainName = Regions.Where(x => x.RegionID == system.RegionID).First().RegionName;
            intelSystem.SystemConstellationName = Constellations.Where(x => x.ConstellationID == system.ConstellationID).First().ConstallationName;

            foreach (MapSolarSystemJump connection in SolarSystemJumps.Where(x => x.FromSolarSystemID == intelSystem.SystemId))
            {
                intelSystem.ConnectedSytemsId.Add(connection.ToSolarSystemID);
            }

            return intelSystem;
        }
    }
}
