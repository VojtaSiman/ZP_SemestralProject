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

    public Tuple<int, int> GetRelicPosition()
    {
        var startPosition = GetStartPosition();
        Random random = new Random();
        
        int attempts = 0;

        while (attempts < 200)
        {
            int x = random.Next(0, Width);
            int y = random.Next(0, Height);

            if (
                IsInBounds(x, y) &&
                tiles[x, y].Type == TileType.Floor &&
                int.Abs(startPosition.Item1 - x) > Width / 3 &&
                int.Abs(startPosition.Item2 - y) > Height / 2 &&
                x != 0 && x != Width - 1 && y != 0 && y != Height - 1
            )
            {
                return new Tuple<int, int>(x, y);
            }
            
            attempts++;
        }
        
        // Fallback, take first empty tile from the bottom of the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                if (tiles[x, y].Type == TileType.Floor)
                {
                    return new Tuple<int, int>(x, y);
                }
            }
        }
        
        return new Tuple<int, int>(Width - 1, Height - 1);
    }

    public Position GetNpcSpawnPosition()
    {
        var startPosition = GetStartPosition();
        Random random = new Random();
        
        int attempts = 0;

        while (attempts < 200)
        {
            int x = random.Next(0, Width);
            int y = random.Next(0, Height);

            if (
                IsInBounds(x, y) &&
                IsPassable(x, y) &&
                tiles[x, y].Type == TileType.Floor &&
                int.Abs(startPosition.Item1 - x) > Width / 3 &&
                int.Abs(startPosition.Item2 - y) > Height / 2
            )
            {
                return new Position(x, y);
            }
            
            attempts++;
        }

        // Fallback, take first empty tile from the bottom of the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                if (tiles[x, y].Type == TileType.Floor)
                {
                    return new Position(x, y);
                }
            }
        }
        
        // Impossible fallback but I am keeping it here for debugging purposes
        return new Position(Width - 1, Height - 1);
    }
}