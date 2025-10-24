namespace zpsem;

public class Game
{
    private World world;
    private Random random;
    private Renderer renderer;
    private Player player;
    private List<NPC> npcs;
    private List<Item> items;
    private bool isRunning;
    private string message;

    private static int worldWidth = 48;
    private static int worldHeight = 20;

    private static int numberOfItems = 10;
    private List<bool> itemLuckList;
    private int itemLuck = 9;
    
    public Game()
    {
        random = new Random();
        
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
            player.Energy -= 1;
            bool foundItem = false;

            // Checking if we picked up an item
            foreach (Item item in items.ToList())
            {
                if (player.X == item.X && player.Y == item.Y)
                {
                    items.Remove(item);
                    player.Energy += 20;
                    foundItem = true;
                }
            }
            
            if (foundItem) message = "> You gained 20 energy!";
            else if (directionX == -1 && directionY == 0) message = "> You moved left.";
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
                bool isWallDestroyed = world.GetTile(player.X + directionX, player.Y + directionY).DamageWall(1);
                if (isWallDestroyed) message = "> The wall has broken!";
                else message = "> You hit the wall. It got a bit weaker!";
                
                player.Energy -= 1;

                // Small chance that we will find an energy item after destroying a wall
                if (
                    isWallDestroyed &&
                    DrawLuckDeck()
                    )
                {
                    items.Add(new Item(player.X + directionX, player.Y + directionY, ConsoleColor.Green, '*'));
                }
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
        foreach (var item in items)
        {
            renderer.DrawEntity(item);
        }
        renderer.DrawEntity(player);
        renderer.DrawMessage(message);
        renderer.DrawUI(player);
    }

    private void InitializeWorld()
    {
        world = new World(worldWidth, worldHeight);
        WorldGenerator.Generate(world);

        npcs = new List<NPC>();
        items = new List<Item>();
        itemLuckList = new List<bool>();
        
        int itemCounter = 0;

        while (itemCounter < numberOfItems)
        {
            int x = random.Next(0, worldWidth);
            int y = random.Next(0, worldHeight);

            if (world.GetTile(x, y).Type == TileType.Floor)
            {
                items.Add(new Item(x, y, ConsoleColor.Green, '*'));
                itemCounter++;
            }
        }

        
        
        var startPosition = world.GetStartPosition();
        player = new Player(startPosition.Item1, startPosition.Item2, ConsoleColor.DarkMagenta, '@');
        player.Energy = 100;
        player.Inventory = new Inventory();
    }

    private bool DrawLuckDeck()
    {
        if (itemLuckList.Count <= 0)
        {
            ShuffleLuckDeck();
        }

        bool answer = itemLuckList.First();
        itemLuckList.RemoveAt(0);
        return answer;
    }
    private void ShuffleLuckDeck()
    {
        for (int i = 0; i < itemLuck - 1; i++)
        {
            itemLuckList.Add(false);
        }
        itemLuckList.Add(true);
        Shuffle(itemLuckList);
    }

    // Fisher-Yates Shuffle
    // Taken from:
    // https://jamesshinevar.medium.com/shuffle-a-list-c-fisher-yates-shuffle-32833bd8c62d
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}