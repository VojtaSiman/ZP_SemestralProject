namespace zpsem;

public enum TileType
{
    Wall,
    Floor
}

public class Tile
{
    public TileType Type { get; set; }
    public bool IsSolid=> Type == TileType.Wall;
    public char Glyph => Type == TileType.Wall ? '#' : '.';

    public Tile(TileType type)
    {
        Type = type;
    }
}