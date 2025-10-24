namespace zpsem;

public class Game
{
    private World world;
    private Random random;
    private Renderer renderer;
    private Player player;
    private List<NPC> npcs;
    private bool isRunning;
    
    public Game()
    {
        world = new World(12, 8);

        player = new Player(5, 1, ConsoleColor.DarkMagenta, '@');
        
        renderer = new Renderer();
        isRunning = true;
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
        }
        
        if (world.IsPassable(player.X + directionX, player.Y + directionY))
        {
            player.Move(directionX, directionY);    
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
        renderer.DrawUI();
    }
}