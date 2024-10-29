using ChoETL;
using System.Text;

string solarSystemJumpsCSV = File.ReadAllText("mapSolarSystemJumps.csv");
string solarSystemCSV = File.ReadAllText("mapSolarSystems.csv");

StringBuilder solarSystemsSB = new StringBuilder();
using (ChoCSVReader<dynamic> p = ChoCSVReader.LoadText(solarSystemCSV).WithField("solarSystemID", 3).WithField("solarSystemName", 4).WithFirstLineHeader(true))
using (ChoJSONWriter w = new ChoJSONWriter(solarSystemsSB))
    w.Write(p);

StringBuilder solarSystemJumpsSB = new StringBuilder();
using (ChoCSVReader<dynamic> p = ChoCSVReader.LoadText(solarSystemJumpsCSV).WithField("fromSolarSystemID", 3).WithField("toSolarSystemID", 4).WithFirstLineHeader(true))
using (ChoJSONWriter w = new ChoJSONWriter(solarSystemJumpsSB))
    w.Write(p);

File.WriteAllText("mapSolarSystems.json", solarSystemsSB.ToString());
File.WriteAllText("mapSolarSystemJumps.json", solarSystemJumpsSB.ToString());

