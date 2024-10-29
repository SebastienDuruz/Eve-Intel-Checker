using EveIntelCheckerLib.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EveIntelCheckerLib.Data;

/// <summary>
/// Class LinuxSettingsReader
/// </summary>
public class LinuxSettingsReader
{
    /// <summary>
    /// File path of the userSettings file
    /// </summary>
    private string FilePath { get; }

    /// <summary>
    /// Objects that contains the linux values
    /// </summary>
    public LinuxSettings LinuxSettingsValues { get; set; }

    /// <summary>
    /// Custom Constructor
    /// </summary>
    public LinuxSettingsReader()
    {
        if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker")))
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"));

        FilePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EveIntelChecker"), $"linuxSettings.json");

        LinuxSettingsValues = new LinuxSettings();
        ReadLinuxSettings();
    }

    /// <summary>
    /// Read the linuxSettings json file, create it if not exists
    /// Load the content into UserSettings Object
    /// </summary>
    public void ReadLinuxSettings()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                LinuxSettingsValues = JsonConvert.DeserializeObject<LinuxSettings>(File.ReadAllText(FilePath))!;
            }
            catch (Exception ex)
            {
                StaticData.Log(StaticData.LogLevel.Warning, ex.Message);

                // Reset the settings by recreating a file
                WriteLinuxSettings();
                LinuxSettingsValues = new LinuxSettings();
            }
        }
        else
        {
            LinuxSettingsValues = new LinuxSettings();
            WriteLinuxSettings();
        }
    }

    /// <summary>
    /// Write the LinuxSettings object to json file
    /// </summary>
    public void WriteLinuxSettings()
    {
        try
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(LinuxSettingsValues, Formatting.Indented));
        }
        catch (Exception ex)
        {
            StaticData.Log(StaticData.LogLevel.Warning, ex.Message);
        }
    }
}