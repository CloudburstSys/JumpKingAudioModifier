using System;
using System.Collections.Generic;
using Logging;
using Logging.API;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JumpKingAudioModifier.Settings;
using Microsoft.VisualBasic;

namespace JumpKingAudioModifier.Helpers;

public static class FileHelper
{

    public static string GetPath(string file)
    {
        return $"{Directory.GetCurrentDirectory()}\\{file.Replace("/","\\")}";
    } 
    
    public static string GetFilePath(string name, string ext)
    {
        return GetPath($"{name}.{ext}");
    }
    
    public static string GetFilePath(string path, string name, string ext)
    {
        return GetPath($"{path.Replace("/","\\")}\\{name}.{ext}");
    }

    public static List<string> GetAllFilesInDirectory(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException();

        return Directory.EnumerateFiles(path).ToList();
    }

    public static string GetRandomFilePath(string path)
    {
        var files = GetAllFilesInDirectory(path);

        var R = new Random();
        var index = R.Next(0, files.Count);

        return files[index];
    }

    public static List<SFXPresetDefinition> LoadExternalPresets(ILogger logger)
    {
        //return new List<SFXPresetDefinition>();
        var presetList = new List<SFXPresetDefinition>();

        if (!Directory.Exists(GetPath("Presets")))
            Directory.CreateDirectory(GetPath("Presets"));

        var directories = Directory.EnumerateDirectories(GetPath("Presets"));
        
        foreach (var directory in directories)
        {
            logger.Information($"{directory}\\metadata.json");
            // This is potentially a preset. Check for the existence of "metadata.json".
            if (File.Exists($"{directory}\\metadata.json"))
            {
                
                // It exists! This has to be a preset folder. Digest the metadata.json file.
                var preset = new SFXPresetDefinition();
                
                logger.Information($"Loading preset in preset file {directory}.");
                
                var fileContents = File.ReadAllText($"{directory}\\metadata.json");
                preset = JsonConvert.DeserializeObject<SFXPresetDefinition>(fileContents);
                
                if (preset == null)
                {
                    // Failed to load. Warn in console and abort.
                    logger.Warning($"Failed to load preset in preset file {directory} due to invalid metadata.json.");
                    continue;
                }
                
                // Set preset directory if not already set.
                if (preset.Directory == "")
                    preset.Directory = directory;
                else preset.Directory = $"{directory}{preset.Directory}";
                
                presetList.Add(preset);
                logger.Information($"Loaded preset \"{preset.Name}\".");
            }
        }

        return presetList;
    }
    
    public static Settings.Settings LoadSettings(ILogger logger)
    {
        var settings = new Settings.Settings(); // Sets the default configuration values for now.
    
        logger.Information(GetFilePath("Settings", "json"));
        
        if (!File.Exists(GetFilePath("Settings", "json")))
        {
            // Convert default UserSettings into JSON using JsonConvert and save file.
            logger.Information("Couldn't find config file.");
            var defaultFileContents = JsonConvert.SerializeObject(settings, Formatting.Indented).Replace("\r\n", "\n"); // Normalise some stuff
            File.WriteAllText(GetFilePath("Settings", "json"), defaultFileContents);
        }
        else
        {
            // File found, read and deseralize into a UserSettings which we store as settings.
            logger.Information("Found config");
            var fileContents = File.ReadAllText(GetFilePath("Settings", "json"));
            settings = JsonConvert.DeserializeObject<Settings.Settings>(fileContents);
            logger.Information(JsonConvert.SerializeObject(settings, Formatting.Indented).Replace("\r\n", "\n"));
            if (settings == null)
            {
                throw new InvalidDataException($"Failed to deserialize settings at {GetFilePath("Settings", "json")}");
            }
        }

        return settings;
    }

    public static void SaveSettings(Settings.Settings settings)
    {
        var fileContents = JsonConvert.SerializeObject(settings, Formatting.Indented).Replace("\r\n", "\n"); // Normalise some stuff
        File.WriteAllTextAsync(GetFilePath("Settings", "json"), fileContents);
    }

    public static void DuplicateFile(string sourcePath, string destinationPath)
    {
        if(File.Exists(destinationPath))
            File.Delete(destinationPath);
        
        File.Copy(sourcePath, destinationPath);
    }
}