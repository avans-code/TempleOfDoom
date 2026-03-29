using TempleOfDoom.Data.Loaders;
using TempleOfDoom.Data.Factories;
using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Controllers;
using TempleOfDoom.UI.Renderers;

namespace TempleOfDoom.UI;

class Program
{
    static void Main()
    {
        try
        {
            // 1. Data inladen en omzetten
            var loader = new JsonLevelLoader();
            var levelDto = loader.LoadLevel("TempleOfDoom_Extended_C_2223.json");
            var factory = new LevelFactory();
            Level level = factory.CreateLevel(levelDto);

            // 2. Controller en Renderer starten
            var gameController = new GameController(level);
            var renderer = new ConsoleRenderer();

            // 3. De Game Loop (Blijf draaien tot je dood bent of wint)
            while (!gameController.IsGameOver)
            {
                renderer.Render(gameController.CurrentLevel);
                
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // true = verberg de aangeslagen letter
                gameController.HandleInput(keyInfo.Key);
            }

            // Game Over Scherm
            renderer.Render(gameController.CurrentLevel);
            Console.WriteLine("\n=============================");
            Console.WriteLine(level.Player.StonesCollected >= 5 ? "Gefeliciteerd, je hebt gewonnen!" : "Game Over!");
            Console.WriteLine("=============================");
            
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Er ging iets mis:\n{ex.Message}");
        }
    }
}