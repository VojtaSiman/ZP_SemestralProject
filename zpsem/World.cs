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
                // Add borders around the level
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tiles[x, y] = new Tile(TileType.Wall);
                }
                else
                {
                    tiles[x, y] = new Tile(TileType.Floor);
                }
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        return IsInBounds(x, y) ? tiles[x, y] : new Tile(TileType.Wall);
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (IsInBounds(x, y))
        {
            tiles[x, y] = new Tile(type);
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsPassable(int x, int y)
    {
        return IsInBounds(x, y) && !GetTile(x, y).IsSolid;
    }

    public Tuple<int, int> GetStartPosition()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (IsPassable(x, y)) return new Tuple<int, int>(x, y);
            }
        }
        
        return new Tuple<int, int>(Width / 2, Height / 2);
    }
}