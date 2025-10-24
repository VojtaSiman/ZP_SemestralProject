namespace zpsem;

public class Game
{
    private World world;
    private Random random;
    private Renderer renderer;
    private Player player;
    private List<NPC> npcs;
    private bool isRunning;
    private string message;

    private int worldWidth = 48;
    private int worldHeight = 20;
    
    public Game()
    {
        InitializeWorld();
        
        renderer = new Renderer();
        isRunning = true;
        message = ">";
    }

    public void Run()
    {
        Render();
        
        while (isRunning)
        {
            ProcessInput();
            Update();
            Render();
        }
    }

    private void ProcessInput()
    {
        var key = Console.ReadKey(true).Key;

        int directionX = 0;
        int directionY = 0;
        
        switch (key)
        {
            case ConsoleKey.W:
                // Walk up
                directionY = -1;
                break;
            case ConsoleKey.S:
                // Walk down
                directionY = 1;
                break;
            case ConsoleKey.A:
                // Walk left
                directionX = -1;
                break;
            case ConsoleKey.D:
                // Walk right
                directionX = 1;
                break;
            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                isRunning = false;
                break;
            case ConsoleKey.Spacebar:
                // Wait one step
                break;
            case ConsoleKey.R:
                // Regenerate the world
                InitializeWorld();
                break;
        }
        
        if (world.IsPassable(player.X + directionX, player.Y + directionY))
        {
            player.Move(directionX, directionY);
            
            if (directionX == -1 && directionY == 0) message = "> You moved left.";
            else if (directionX == 1 && directionY == 0) message = "> You moved right.";
            else if (directionX == 0 && directionY == -1) message = "> You moved up.";
            else if (directionX == 0 && directionY == 1) message = "> You moved down.";
        }
        else
        {
            if (
                world.GetTile(player.X + directionX, player.Y + directionY).Type == TileType.Wall &&
                world.IsInBounds(player.X + directionX, player.Y + directionY)
                )
            {
                bool response = world.GetTile(player.X + directionX, player.Y + directionY).DamageWall(1);
                if (response) message = "> The wall has broken!";
                else message = "> You hit the wall. It got a bit weaker!";
            }
            else
            {
                message = "> Cannot move in this direction.";
            }
        }
    }
    
    private void Update()
    {
        
    }

    private void Render()
    {
        renderer.Clear();
        renderer.DrawWorld(world);
        renderer.DrawEntity(player);
        renderer.DrawMessage(message);
        renderer.DrawUI();
    }

    private void InitializeWorld()
    {
        world = new World(worldWidth, worldHeight);
        WorldGenerator.Generate(world);

        var startPosition = world.GetStartPosition();
        player = new Player(startPosition.Item1, startPosition.Item2, ConsoleColor.DarkMagenta, '@');
    }
}