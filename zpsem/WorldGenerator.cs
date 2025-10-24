namespace zpsem;

// Inspired by this article:
// https://www.gridbugs.org/cellular-automata-cave-generation/
public class WorldGenerator
{
    private static float DENSITY = 0.55f;
    private static int SURVIVE_MIN = 4;
    private static int SURVIVE_MAX = 8;
    private static int RESURRECT_MIN = 5;
    private static int RESURRECT_MAX = 5;
    private static int MIN_CAVE_SIZE = 10;
    private static Random random = new Random();

    public static void Generate(World world)
    {
        // Filling the world with random noise
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                if (random.NextDouble() < DENSITY)
                {
                    world.SetTile(x, y, TileType.Wall);    
                }
                else
                {
                    world.SetTile(x, y, TileType.Floor);
                }
                
            }
        }
        
        AddBorder(world);
        
        // Cellular automata - doing several steps
        for (int i = 0; i < 3; i++)
        {
            CellularAutomataStep(world);
            AddBorder(world);
        }
        
        CleanupWalls(world);
        RemoveSmallCaves(world, MIN_CAVE_SIZE);
    }

    public static void AddBorder(World world)
    {
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                // Add borders around the level
                if (x == 0 || y == 0 || x == world.Width - 1 || y == world.Height - 1)
                {
                    world.SetTile(x, y, TileType.Wall);
                }
            }
        }
    }

    public static void CellularAutomataStep(World world)
    {
        Tile[,] newTiles = new Tile[world.Width, world.Height];
        
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                int countNeighborWalls = CountNeighborWalls(world, x, y);

                if (world.GetTile(x, y).Type == TileType.Wall)
                {
                    newTiles[x, y] = countNeighborWalls >= SURVIVE_MIN && countNeighborWalls <= SURVIVE_MAX
                        ? new Tile(TileType.Wall)
                        : new Tile(TileType.Floor);
                }
                else
                {
                    newTiles[x, y] = countNeighborWalls >= RESURRECT_MIN && countNeighborWalls <= RESURRECT_MAX
                        ? new Tile(TileType.Wall)
                        : new Tile(TileType.Floor);
                }
            }
        }

        // Apply changes
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                world.SetTile(x, y, newTiles[x, y].Type);
            }
        }
    }

    private static int CountNeighborWalls(World world, int x, int y)
    {
        int count = 0;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue; // Ignore self
                
                if (world.GetTile(x + dx, y + dy).Type == TileType.Wall)
                {
                    count++;
                }
            }
        }
        return count;
    }

    // Gets rid of small gaps or walls
    private static void CleanupWalls(World world)
    {
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                int countNeighborWalls = CountNeighborWalls(world, x, y);
                
                if (world.GetTile(x, y).Type == TileType.Wall && countNeighborWalls < 2)
                {
                    world.SetTile(x, y, TileType.Floor);
                }
                
                if (world.GetTile(x, y).Type == TileType.Floor && countNeighborWalls > 5)
                {
                    world.SetTile(x, y, TileType.Wall);
                }
            }
        }
    }

    private static void RemoveSmallCaves(World world, int minSize = 10)
    {
        bool[,] visited = new bool[world.Width, world.Height];

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                if (!visited[x, y] && world.GetTile(x, y).Type == TileType.Floor)
                {
                    // We found an unvisited tile, let's explore its neighbors with a flood fill algorithm
                    List<Position> cave = FloodFill(world, x, y, visited);

                    // If the cave is smaller than the min size, fill it with walls
                    if (cave.Count < minSize)
                    {
                        foreach (var pos in cave)
                        {
                            world.SetTile(pos.X, pos.Y, TileType.Wall);
                        }
                    }
                }
            }
        }
    }

    private static List<Position> FloodFill(World world, int x, int y, bool[,] visited)
    {
        List<Position> cave = new List<Position>();
        Queue<Position> queue = new Queue<Position>();
        
        // Starting the breadth first search
        queue.Enqueue(new Position(x, y));
        visited[x, y] = true;

        while (queue.Count > 0)
        {
            Position current = queue.Dequeue();
            cave.Add(current);
            
            // Four neighbors of current tile
            Position[] neighbors =
            {
                new(current.X - 1, current.Y),
                new(current.X + 1, current.Y),
                new(current.X, current.Y - 1),
                new(current.X, current.Y + 1)
            };
            
            // Check each neighbor
            // 1. It should be in bounds of the world array
            // 2. It should be a tile we have not visited yet
            // 3. It has to be an empty (floor) tile
            foreach (Position neighbor in neighbors)
            {
                if (
                    world.IsInBounds(neighbor.X, neighbor.Y) &&
                    !visited[neighbor.X, neighbor.Y] &&
                    world.GetTile(neighbor.X, neighbor.Y).Type == TileType.Floor
                    )
                {
                    // Mark as visited, add to queue for further exploration
                    visited[neighbor.X, neighbor.Y] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        // We are returning the list of all the adjacent neighbors
        // Thank you flood fill algorithm (yay, finally had a reason to implement it)
        return cave;
    }
}