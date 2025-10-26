namespace zpsem;

public enum TileType
{
    Wall,
    Floor,
    Relic,
    Start
}

public class Tile
{
    public TileType Type { get; set; }
    public bool IsSolid => Type == TileType.Wall;
    public char Glyph
    {
        get
        {
            if (Type == TileType.Wall)
            {
                if (WallHealth >= 3) return '█';
                if (WallHealth >= 2) return '▒';
                if (WallHealth >= 1) return '░';
            }
            else if (Type == TileType.Floor)
            {
                return '.';
            }
            else if (Type == TileType.Relic)
            {
                return '$';
            }
            else if (Type == TileType.Start)
            {
                return '+';
            }
            return ' ';
        }
    }
    
    public ConsoleColor Color { get; set; }
    
    public int WallHealth { get; set; }
    private Random random = new Random();
    
    public bool IsExplored { get; set; }
    
    public Tile(TileType type)
    {
        // IsExplored = true; // Starts with world tiles visible
        Type = type;
        if (type == TileType.Wall)
        {
            if (random.NextDouble() < 0.7)
            {
                WallHealth = 2;
            }
            else
            {
                WallHealth = 3;
            }
            
            Color = ConsoleColor.DarkGray;
        }
        else if (type == TileType.Relic)
        {
            WallHealth = 0;
            Color = ConsoleColor.Yellow;
        }
        else if (type == TileType.Start)
        {
            Color = ConsoleColor.Magenta;
        }
        else
        {
            WallHealth = 0;
            Color = ConsoleColor.DarkGreen;
        }
    }

    public bool DamageWall(int health = 1)
    {
        if (Type != TileType.Wall) return false;
        
        WallHealth -= health;

        // Destroying the wall
        if (WallHealth <= 0)
        {
            Type = TileType.Floor;
            WallHealth = 0;
            return true;
        }
        
        return false;
    }
}