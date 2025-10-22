namespace zpsem;

public class Game
{
    private World world;
    private Random random;
    private Renderer renderer;
    private InputHandler inputHandler;
    private Player player;
    private List<NPC> npcs;
    private bool isRunning;
    
    public Game()
    {
        world = new World(12, 8);
        
        renderer = new Renderer();
        inputHandler = new InputHandler();
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
        
    }
    
    private void Update()
    {
        
    }

    private void Render()
    {
        renderer.Clear();
        renderer.DrawWorld(world);
    }
}