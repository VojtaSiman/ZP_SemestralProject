namespace zpsem;

public abstract class Entity(int x, int y, ConsoleColor color, char glyph)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public ConsoleColor Color { get; set; } = color;
    public char Glyph { get; set; } = glyph;

    public void Move(int deltaX, int deltaY)
    {
        X += deltaX;
        Y += deltaY;
    }
}