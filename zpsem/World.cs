namespace zpsem;

public class World
{
    private Tile[,] tiles;
    public int Width { get; }
    public int Height { get; }

    public World(int width, int height)
    {
        Width = width;
        Height = height;
        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(TileType.Floor);
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        return IsInBounds(x, y) ? tiles[x, y] : new Tile(TileType.Floor);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}