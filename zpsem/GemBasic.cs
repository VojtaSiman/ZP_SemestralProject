namespace zpsem;

public class GemBasic(int x, int y, ConsoleColor color = ConsoleColor.Blue, char glyph = '#') : Entity(x, y, color, glyph)
{
    public int Value = 1000;
}