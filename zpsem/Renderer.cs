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
                Console.Write(tile.Glyph);
            }
            
            Console.WriteLine();
        }
    }
}