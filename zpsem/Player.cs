namespace zpsem;

public class Player(int x, int y, ConsoleColor color, char glyph) : Entity(x, y, color, glyph)
{
    public int Energy {get; set;}
    public Inventory Inventory = new Inventory();
}