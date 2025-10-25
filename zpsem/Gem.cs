namespace zpsem;

public class Gem(int x, int y, ConsoleColor color, char glyph, int value, string name) : Entity(x, y, color, glyph)
{
    public int Value = value;
    public string Name = name;
}