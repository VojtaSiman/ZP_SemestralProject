namespace zpsem;

public class Game
{
    private World _world = null!;
    private readonly Random _random;
    private Player _player = null!;
    private List<NPC> _npcs = null!;
    private List<EnergyItem> _items = null!;
    private List<Gem> _gems = null!;

    private const int WorldWidth = 48;
    private const int WorldHeight = 20;

    private const int NumberOfItems = 10;
    private List<bool> _itemLuckList = null!;
    private const int ItemLuck = 9;

    private const int NumberOfBasicGems = 20;
    private const int NumberOfRareGems = 7;
    
    private const int DefaultPlayerEnergy = 100;
    
    private bool _isRunning;
    private string _message;
    private int _score;

    public Game()
    {
        _random = new Random();
        
        InitializeWorld();
        
        _isRunning = true;
        _message = ">";
    }

    public void Run()
    {
        Render();
        
        while (_isRunning)
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
                _isRunning = false;
                break;
            case ConsoleKey.Spacebar:
                // Wait one step
                break;
            case ConsoleKey.R:
                // Regenerate the world
                InitializeWorld();
                break;
        }
        
        if (_world.IsPassable(_player.X + directionX, _player.Y + directionY))
        {
            _player.Move(directionX, directionY);
            
            if (directionX != 0 || directionY != 0)
            {
                _player.Energy -= 1;
            }
            
            bool foundEnergyItem = false;
            bool foundGem = false;

            // Checking if we picked up an item
            foreach (EnergyItem item in _items.ToList())
            {
                if (_player.X != item.X || _player.Y != item.Y) continue;
                
                _items.Remove(item);
                _player.Energy += 20;
                foundEnergyItem = true;
            }
            
            // Checking if we picked up a gem
            foreach (var gem in _gems.ToList())
            {
                if (gem.X != _player.X || gem.Y != _player.Y) continue;
                _score += gem.Value;
                _player.Inventory.AddGemToInventory(gem);
                _gems.Remove(gem);
                foundGem = true;
            }
            
            if (foundEnergyItem) _message = "> You gained 20 energy!";
            else if (foundGem) _message = "> You picked up a gem!";
            else if (directionX == -1 && directionY == 0) _message = "> You moved left.";
            else if (directionX == 1 && directionY == 0) _message = "> You moved right.";
            else if (directionX == 0 && directionY == -1) _message = "> You moved up.";
            else if (directionX == 0 && directionY == 1) _message = "> You moved down.";
        }
        else
        {
            if (
                _world.GetTile(_player.X + directionX, _player.Y + directionY).Type == TileType.Wall &&
                _world.IsInBounds(_player.X + directionX, _player.Y + directionY)
                )
            {
                bool isWallDestroyed = _world.GetTile(_player.X + directionX, _player.Y + directionY).DamageWall();
                _message = isWallDestroyed ? "> The wall has broken!" : "> You hit the wall. It got a bit weaker!";
                
                _player.Energy -= 1;

                // Small chance that we will find an energy item after destroying a wall
                if (
                    isWallDestroyed &&
                    DrawLuckDeck()
                    )
                {
                    _items.Add(new EnergyItem(_player.X + directionX, _player.Y + directionY, ConsoleColor.Green, '*'));
                }
            }
            else
            {
                _message = "> Cannot move in this direction.";
            }
        }
    }
    
    private void Update()
    {
        foreach (var npc in _npcs.ToList())
        {
            npc.Update(_world);
        }
    }

    private void Render()
    {
        Renderer.Clear();
        Renderer.DrawWorld(_world);
        foreach (var item in _items)
        {
            Renderer.DrawEntity(item);
        }

        foreach (var gem in _gems)
        {
            Renderer.DrawEntity(gem);
        }
        
        foreach (var npc in _npcs)
        {
            Renderer.DrawEntity(npc);
        }
        Renderer.DrawEntity(_player);
        Renderer.DrawMessage(_message);
        Renderer.DrawUserInterface(_player, _score + _player.Energy);
    }

    private void InitializeWorld()
    {
        _world = new World(WorldWidth, WorldHeight);
        WorldGenerator.Generate(_world);

        _npcs = [];
        _items = [];
        _gems = [];
        _itemLuckList = [];

        // Initialize energy items
        int itemCounter = 0;

        while (itemCounter < NumberOfItems)
        {
            var x = _random.Next(0, WorldWidth);
            var y = _random.Next(0, WorldHeight);

            if (_world.GetTile(x, y).Type != TileType.Floor) continue;
            _items.Add(new EnergyItem(x, y, ConsoleColor.Green, '*'));
            itemCounter++;
        }

        // Initialize gems
        int gemCounter = 0;

        while (gemCounter < NumberOfBasicGems)
        {
            int x = _random.Next(0, WorldWidth);
            int y = _random.Next(0, WorldHeight);
            
            if (_world.GetTile(x, y).Type == TileType.Wall)
            { 
                _gems.Add(new GemBasic(x, y));
                gemCounter++;
            }
        }
        
        gemCounter = 0;

        while (gemCounter < NumberOfRareGems)
        {
            int x = _random.Next(0, WorldWidth);
            int y = _random.Next(0, WorldHeight);

            if (_world.GetTile(x, y).Type != TileType.Wall) continue;
            
            _gems.Add(new GemRare(x, y));
            gemCounter++;
        }

        // Initialize player
        var startPosition = _world.GetStartPosition();
        _world.SetTile(startPosition.Item1, startPosition.Item2, TileType.Start);
        _player = new Player(startPosition.Item1, startPosition.Item2, ConsoleColor.DarkMagenta, '@');
        _player.Energy = DefaultPlayerEnergy;
        _player.Inventory = new Inventory();

        // Initialize relic
        var relicPosition = _world.GetRelicPosition();
        _world.SetTile(relicPosition.Item1, relicPosition.Item2, TileType.Relic);

        // Initialize NPCs
        for (int i = 0; i < 1; i++)
        {
            var npcSpawnPosition = _world.GetNpcSpawnPosition();
            _npcs.Add(new NPC(npcSpawnPosition.X, npcSpawnPosition.Y, ConsoleColor.Red, 'X'));
        }
    }

    // Checks if player has won the game.
    // Conditions:
    // 1. The Relic has been picked up
    // 2. The player has returned back to start
    private bool CheckWinCondition()
    {
        return false;
    }

    // Checks if player has lost the game.
    // Conditions:
    // 1. The player has ran out of energy
    // 2. An NPC has stepped on player's position
    private bool CheckLoseCondition()
    {
        return false;
    }
    
    private bool DrawLuckDeck()
    {
        if (_itemLuckList.Count <= 0)
        {
            ShuffleLuckDeck();
        }

        bool answer = _itemLuckList.First();
        _itemLuckList.RemoveAt(0);
        return answer;
    }
    private void ShuffleLuckDeck()
    {
        for (int i = 0; i < ItemLuck - 1; i++)
        {
            _itemLuckList.Add(false);
        }
        _itemLuckList.Add(true);
        Shuffle(_itemLuckList);
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
            int k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}