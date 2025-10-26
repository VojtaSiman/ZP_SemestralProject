namespace zpsem;

public class NPC(int x, int y, ConsoleColor color, char glyph) : Entity(x, y, color, glyph)
{
    public void Update(World world, Player player)
    {
        Random random = new Random();
        Position[] directions =
        {
            new(-1, 0),
            new(1, 0),
            new(0, -1),
            new(0, 1)
        };

        int dx = player.X - X; 
        int dy = player.Y - Y;
        int distanceFromPlayer = int.Abs(dx) + int.Abs(dy); // Manhattan distance
        
        // Random chance for the NPC to move in a random direction
        if (random.NextDouble() < 0.55)
        {
            Position direction = directions[0];
            
            // If player is nearby
            if (distanceFromPlayer < 8)
            {
                // Check if player is further away horizontally or vertically
                if (int.Abs(dx) > int.Abs(dy))
                {
                    // Move horizontally in the direction of player
                    direction = new(Math.Sign(dx), 0);
                }
                else
                {
                    // Move vertically in the direction of player
                    direction = new(0, Math.Sign(dy));
                }
            }
            
            // If the path forward is blocked or the player is too far, let's pick a random direction in unblocked direction.
            if (distanceFromPlayer >= 8 || !world.IsPassable(X + direction.X, Y + direction.Y))
            {
                List<Position> possibleDirections = [];
                foreach (var dir in directions)
                {
                    if (world.IsPassable(X + dir.X, Y + dir.Y))
                    {
                        possibleDirections.Add(dir);
                    }
                }
                direction = possibleDirections[random.Next(0, possibleDirections.Count)];
            }
            
            // Renderer.StoreDebugLine("Player " + player.X + "," + player.Y + " dx: " + dx + "  dy: " + dy + "  distanceFromPlayer: " + distanceFromPlayer + "  direction: " + direction.X + "," + direction.Y);
            
            if (world.IsPassable(X + direction.X, Y + direction.Y))
            {
                Move(direction.X, direction.Y);
            }
        }
    }
}