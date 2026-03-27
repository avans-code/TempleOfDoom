using System.Text.Json;
using TempleOfDoom.Data.DTOs;

namespace TempleOfDoom.Data.Loaders;

public class JsonLevelLoader
{
    public LevelDTO LoadLevel(string fileName)
    {
        // Het pad is relatief aan de 'bin' map waar je app draait
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Levels", fileName);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Level bestand niet gevonden op: {fullPath}");
        }

        string jsonString = File.ReadAllText(fullPath);
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var levelData = JsonSerializer.Deserialize<LevelDTO>(jsonString, options);

        return levelData ?? throw new Exception("JSON kon niet worden geparsed.");
    }
}