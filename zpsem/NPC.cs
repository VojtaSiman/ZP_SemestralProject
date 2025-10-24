namespace zpsem;

public class NPC(int x, int y, ConsoleColor color, char glyph) : Entity(x, y, color, glyph)
{
    public void Update(World world)
    {
        Random random = new Random();
        Position[] directions =
        {
            new(-1, 0),
            new(1, 0),
            new(0, -1),
            new(0, 1)
        };

        // Random chance for the NPC to move in a random direction
        if (random.NextDouble() < 0.3)
        {
            Position direction = directions[random.Next(0, directions.Length)];

            if (world.IsPassable(X + direction.X, Y + direction.Y))
            {
                Move(direction.X, direction.Y);
            }
        }
    }
}