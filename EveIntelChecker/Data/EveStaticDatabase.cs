/// Author : Sébastien Duruz
/// Date : 23.08.2022

using EveIntelChecker.Models;
using Microsoft.Data.Sqlite;
using System.Data;

namespace EveIntelChecker.Data
{
    public class EveStaticDatabase
    {
        /// <summary>
        /// Connection object for SQLite Database
        /// </summary>
        private SqliteConnection SqliteConnection { get; set; }

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
            SqliteConnection = CreateConnection();

            SolarSystems = this.ReadSolarSystems();
            SolarSystemJumps = this.ReadSolarSystemsJumps();
            Regions = this.ReadRegions();
            Constellations = this.ReadConstellations();
        }

        /// <summary>
        /// Establish the connection with the SQLite Database
        /// </summary>
        /// <returns>An object that contains the connection to Sqlite DB</returns>
        private SqliteConnection CreateConnection()
        {
            SqliteConnection sqlite_conn = new SqliteConnection("Data Source=eve-static-data.sqlite;");
            try { sqlite_conn.Open(); }
            catch (Exception) { }
            return sqlite_conn;
        }

        /// <summary>
        /// Read the SolarSystems from Sqlite DB
        /// </summary>
        /// <returns>List of SolarSystems</returns>
        private List<MapSolarSystem> ReadSolarSystems()
        {
            List<MapSolarSystem> solarSystems = new List<MapSolarSystem>();
            string query = @"SELECT regionID, constellationID, solarSystemID, solarSystemName FROM mapSolarSystems";
            
            SqliteCommand command = this.SqliteConnection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                solarSystems.Add(new MapSolarSystem()
                {
                    RegionID = (Int64)reader["regionID"],
                    ConstellationID = (Int64)reader["constellationID"],
                    SolarSystemID = (Int64)reader["solarSystemID"],
                    SolarSystemName = (string)reader["solarSystemName"],
                });
            }

            return solarSystems;
        }

        /// <summary>
        /// Read the JumpGates from Sqlite DB
        /// </summary>
        /// <returns>List of JumpGates</returns>
        private List<MapSolarSystemJump> ReadSolarSystemsJumps()
        {
            List<MapSolarSystemJump> solarSystemJumps = new List<MapSolarSystemJump>();
            string query = @"SELECT fromSolarSystemID, toSolarSystemID FROM mapSolarSystemJumps";

            SqliteCommand command = this.SqliteConnection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                solarSystemJumps.Add(new MapSolarSystemJump()
                {
                    FromSolarSystemID = (Int64)reader["fromSolarSystemID"],
                    ToSolarSystemID = (Int64)reader["toSolarSystemID"],
                });
            }

            return solarSystemJumps;
        }

        /// <summary>
        /// Read the Regions from Sqlite DB
        /// </summary>
        /// <returns>List of Regions</returns>
        private List<MapRegion> ReadRegions()
        {
            List<MapRegion> regions = new List<MapRegion>();
            string query = @"SELECT regionID, regionName FROM mapRegions";

            SqliteCommand command = this.SqliteConnection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                regions.Add(new MapRegion()
                {
                    RegionID = (Int64)reader["regionID"],
                    RegionName = (String)reader["regionName"],
                });
            }

            return regions;
        }

        /// <summary>
        /// Read the Constellations from Sqlite DB
        /// </summary>
        /// <returns>List of Constellations</returns>
        private List<MapConstellation> ReadConstellations()
        {
            List<MapConstellation> constellations = new List<MapConstellation>();
            string query = @"SELECT regionID, constellationID, constellationName FROM mapConstellations";

            SqliteCommand command = this.SqliteConnection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                constellations.Add(new MapConstellation()
                {
                    RegionID = (Int64)reader["regionID"],
                    ConstellationID = (Int64)reader["constellationID"],
                    ConstallationName = (string)reader["constellationName"],
                });
            }

            return constellations;
        }
    }
}
