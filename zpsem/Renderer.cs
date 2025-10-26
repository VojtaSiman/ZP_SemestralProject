namespace zpsem;

public static class Renderer
{
    private static string _debugMessage = "";
    
    public static void Clear()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    public static void DrawWorld(World world)
    {
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                Tile tile = world.GetTile(x, y);
                Console.ForegroundColor = tile.Color;
                Console.Write(tile.Glyph);
                Console.ResetColor();
            }
            
            Console.WriteLine();
        }
    }

    public static void DrawEntity(Entity entity)
    {
        Console.SetCursorPosition(entity.X, entity.Y);
        Console.ForegroundColor = entity.Color;
        Console.Write(entity.Glyph);
        Console.ResetColor();
    }

    public static void DrawMessage(string message)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 4);
        Console.Write(message);
    }
    
    public static void DrawUserInterface(Player player, int score)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 3);
        
        Console.ForegroundColor = player.Color;
        Console.Write(player.Inventory.GetInventoryContent());
        Console.ResetColor();
        Console.SetCursorPosition(0, Console.WindowHeight - 2);

        Console.ForegroundColor = player.Energy switch
        {
            > 100 => ConsoleColor.Green,
            > 50 => ConsoleColor.DarkGreen,
            > 20 => ConsoleColor.DarkYellow,
            _ => ConsoleColor.DarkRed
        };
        Console.Write("Player energy: " + player.Energy);
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" | Score: " + score);
        
        Console.ResetColor();
        Console.Write(" | Press Q or Esc to quit.");
    }
    
    public static void DrawWinScreen(World world)
    {
        string line1 = "▖▖         ▘  ▌";
        string line2 = "▌▌▛▌▌▌  ▌▌▌▌▛▌▌";
        string line3 = "▐ ▙▌▙▌  ▚▚▘▌▌▌▖";

        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2 - 2);
        Console.Write(line1);
        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2 - 1);
        Console.Write(line2);
        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2);
        Console.Write(line3);
        
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
    }

    public static void DrawGameOverScreen(World world)
    {
        string line1 = "▖▖▄▖▖▖  ▄ ▄▖▄▖▄ ";
        string line2 = "▌▌▌▌▌▌  ▌▌▐ ▙▖▌▌";
        string line3 = "▐ ▙▌▙▌  ▙▘▟▖▙▖▙▘";

        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2 - 2);
        Console.Write(line1);
        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2 - 1);
        Console.Write(line2);
        Console.SetCursorPosition(world.Width / 2 - 8, world.Height / 2);
        Console.Write(line3);
        
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
    }

    public static void StoreDebugLine(string line)
    {
        _debugMessage = line;
    }
    
    public static void DrawDebugLine()
    {
        if (_debugMessage == "") return;
        Console.SetCursorPosition(0, Console.WindowHeight - 5);
        Console.Write(_debugMessage);
    }
}