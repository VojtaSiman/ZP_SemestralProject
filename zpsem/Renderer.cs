namespace zpsem;

public class Renderer
{
    public void Clear()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    public void DrawWorld(World world)
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

    public void DrawEntity(Entity entity)
    {
        Console.SetCursorPosition(entity.X, entity.Y);
        Console.ForegroundColor = entity.Color;
        Console.Write(entity.Glyph);
        Console.ResetColor();
    }

    public void DrawMessage(string message)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 3);
        Console.Write(message);
    }
    
    public void DrawUI()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.Write("Press Q to quit.");
    }
}