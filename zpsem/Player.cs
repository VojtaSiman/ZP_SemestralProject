namespace zpsem;

class Player(int x, int y, ConsoleColor color, char glyph) : Entity(x, y, color, glyph)
{
    public Inventory Inventory = new Inventory();
}