namespace zpsem;

public struct Position
{
    public int X {get; set;}
    public int Y {get; set;}

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.X + b.X, a.Y + b.Y);
    }
}