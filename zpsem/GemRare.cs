namespace zpsem;

public class GemRare(int x, int y, ConsoleColor color = ConsoleColor.Cyan, char glyph = '#') : Entity(x, y, color, glyph)
{
    public int Value = 2500;
}