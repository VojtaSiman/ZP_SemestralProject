namespace zpsem;

public class Game
{
    private readonly Random _random;
    private World _world = null!;
    private Player _player = null!;
    private List<NPC> _npcs = null!;
    private List<EnergyItem> _items = null!;
    private List<Gem> _gems = null!;

    private const int WorldWidth = 48;
    private const int WorldHeight = 20;

    private const int NumberOfItems = 10;
    private const int NumberOfBasicGems = 20;
    private const int NumberOfRareGems = 7;
    
    private const int DefaultPlayerEnergy = 100;
    private const int EnergyRefill = 20;
    private const int EnergyCostPerMove = 1;
    private const int EnergyCostWallHit = 1;
    
    private const int PlayerVisionRadius = 6;
    
    private const int WallDestructionLuckOdds = 9;
    private List<bool> _itemLuckList = null!;
    
    private bool _isRunning;
    private bool _wishToQuitGame;
    private string _message;
    private int _score;
    private bool _hasFoundRelic;

    private int TotalScore => _score + _player.Energy;
    
    public Game()
    {
        _random = new Random();
        InitializeWorld();
        _isRunning = true;
        _message = ">";
    }

    public bool Run()
    {
        UpdateVisibility();
        Render();
        
        while (_isRunning)
        {
            ProcessInput();
            Update();
            Render();
        }
        
        return _wishToQuitGame;
    }

    private void ProcessInput()
    {
        var key = Console.ReadKey(true).Key;
        var direction = GetMovementDirection(key);
        
        if (HandleSpecialKeys(key)) return;

        if (direction != (0, 0))
        {
            AttemptMove(direction.x, direction.y);
        }
    }

    private (int x, int y) GetMovementDirection(ConsoleKey key)
    {
        (int x, int y) returnValue;

        switch (key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                // Walk up
                returnValue = (0, -1);
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                // Walk down
                returnValue = (0, 1);
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                // Walk left
                returnValue = (-1, 0);
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                // Walk right
                returnValue = (1, 0);
                break;
            case ConsoleKey.Spacebar:
                // Wait
                returnValue = (0, 0);
                break;
            default:
                returnValue = (0, 0);
                break;
        }
        
        return returnValue;
    }

    private bool HandleSpecialKeys(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                _isRunning = false;
                _wishToQuitGame = true;
                return true;
            case ConsoleKey.R:
                // Regenerate the world
                InitializeWorld();
                return true;
            default:
                return false;
        }
    }

    private void AttemptMove(int dx, int dy)
    {
        int targetX = _player.X + dx;
        int targetY = _player.Y + dy;

        if (_world.IsPassable(targetX, targetY))
        {
            ExecuteMove(dx, dy);
        }
        else
        {
            HandleCollision(targetX, targetY);
        }
    }

    private void ExecuteMove(int dx, int dy)
    {
        _player.Move(dx, dy);

        // If player actually moved in any direction, decrease energy
        if (dx != 0 || dy != 0)
        {
            _player.Energy -= EnergyCostPerMove;
        }

        HandlePickups();
        UpdateMovementMessage(dx, dy);
    }

    private void HandleCollision(int targetX, int targetY)
    {
        if (IsDestructibleWall(targetX, targetY))
        {
            HandleWallDestruction(targetX, targetY);
        }
        else
        {
            _message = "> Cannot move in this direction.";
        }
    }

    private bool IsDestructibleWall(int x, int y)
    {
        return _world.IsInBounds(x, y) &&
               _world.GetTile(x, y).Type == TileType.Wall;
    }

    private void HandleWallDestruction(int x, int y)
    {
        bool isWallDestroyed = _world.GetTile(x, y).DamageWall();
        _message = isWallDestroyed
            ? "> Wall destroyed!"
            : "> You hit the wall. It got a bit weaker!";
        
        _player.Energy -= EnergyCostWallHit;

        if (isWallDestroyed && DrawLuckDeck())
        {
            SpawnEnergyItemAt(x, y);
        }
    }

    private void HandlePickups()
    {
        bool pickedUpItem = TryPickupEnergyItem();
        bool pickedUpGem = TryPickupGem();

        if (pickedUpItem)
        {
            _message = "> You gained " + EnergyRefill + " energy!";
        }
        else if (pickedUpGem)
        {
            _message = "> You picked up a gem!";
        }
    }

    private bool TryPickupEnergyItem()
    {
        foreach (EnergyItem item in _items.ToList())
        {
            if (_player.X != item.X || _player.Y != item.Y) continue;
                
            _items.Remove(item);
            _player.Energy += EnergyRefill;
            return true;
        } 
        return false;
    }

    private bool TryPickupGem()
    {
        foreach (var gem in _gems.ToList())
        {
            if (gem.X != _player.X || gem.Y != _player.Y) continue;
            _score += gem.Value;
            _player.Inventory.AddGemToInventory(gem);
            _gems.Remove(gem);
            return true;
        }
        return false;
    }

    private void UpdateMovementMessage(int dx, int dy)
    {
        if (_message.StartsWith("> You gained") || _message.StartsWith("> You picked up"))
        {
            return; // Don't override pickup messages
        }
        
        _message = GetMovementMessage(dx, dy);
    }

    private string GetMovementMessage(int dx, int dy)
    {
        switch ((dx, dy))
        {
            case (-1, 0):
                return "> You moved left.";
            case (1, 0):
                return "> You moved right.";
            case (0, -1):
                return "> You moved up.";
            case (0, 1):
                return "> You moved down.";
            default:
                return ">";
        }
    }

    private void SpawnEnergyItemAt(int x, int y)
    {
        _items.Add(new EnergyItem(x, y, ConsoleColor.Green, '*'));
    }
    
    private void Update()
    {
        UpdateVisibility();
        
        foreach (var npc in _npcs.ToList())
        {
            npc.Update(_world, _player);
        }

        bool isGameWon = CheckWinCondition();
        bool isGameLost = CheckLoseCondition();

        if (isGameWon)
        {
            _message = "> You won the game!";
            Renderer.Clear();
            Renderer.DrawWorld(_world);
            Renderer.DrawMessage(_message);
            Renderer.DrawWinScreen(_world);
            _isRunning = false;
        }
        else if (isGameLost)
        {
            if (_player.Energy <= 0)
            {
                _message = "> You ran out of energy!";
            }
            else
            {
                _message = "> You were caught!";
            }
            
            Renderer.Clear();
            Renderer.DrawWorld(_world);
            Renderer.DrawMessage(_message);
            Renderer.DrawGameOverScreen(_world);
            _isRunning = false;
        }
    }

    private void UpdateVisibility()
    {
        // Visibility calculation
        _world.UpdateVisibility(_player.X, _player.Y, PlayerVisionRadius);

        UpdateEntityVisibility(_gems);
        UpdateEntityVisibility(_npcs);
        UpdateEntityVisibility(_items);
    }

    private void UpdateEntityVisibility<T>(List<T> entities) where T : Entity
    {
        foreach (var entity in entities)
        {
            if (IsInPlayerVisionRadius(entity.X, entity.Y, PlayerVisionRadius))
            {
                entity.IsExplored = true;
            }
        }
    }

    private bool IsInPlayerVisionRadius(int x, int y, int radius)
    {
        int dx = Math.Abs(x - _player.X);
        int dy = Math.Abs(y - _player.Y);
        return (dx * dx + dy * dy <= radius * radius);
    }
    
    private void Render()
    {
        if (!_isRunning) return;
        
        Renderer.Clear();
        Renderer.DrawWorld(_world);
        
        // Render entities
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
        
        // Render UI
        Renderer.DrawDebugLine();
        Renderer.DrawMessage(_message);
        Renderer.DrawUserInterface(_player, TotalScore);
    }

    private void InitializeWorld()
    {
        _world = new World(WorldWidth, WorldHeight);
        WorldGenerator.Generate(_world);

        _npcs = [];
        _items = [];
        _gems = [];
        _itemLuckList = [];
        _score = 0;
        _hasFoundRelic = false;

        SpawnRelic();
        SpawnEnergyItems();
        SpawnGems();
        SpawnPlayer();
        SpawnNPCs();
    }

    private void SpawnRelic()
    {
        // Initialize relic
        var relicPosition = _world.GetRelicPosition();
        _world.SetTile(relicPosition.Item1, relicPosition.Item2, TileType.Relic);
    }
    
    private void SpawnEnergyItems()
    {
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
    }

    private void SpawnGems()
    {
        // Initialize gems
        int gemCounter = 0;
        var usedPositions = new HashSet<(int x, int y)>();

        while (gemCounter < NumberOfBasicGems)
        {
            int x = _random.Next(0, WorldWidth);
            int y = _random.Next(0, WorldHeight);

            if (_world.GetTile(x, y).Type != TileType.Wall) continue;
            if (usedPositions.Contains((x, y))) continue;
            
            _gems.Add(new GemBasic(x, y));
            usedPositions.Add((x, y));
            gemCounter++;
        }
        
        gemCounter = 0;

        while (gemCounter < NumberOfRareGems)
        {
            int x = _random.Next(0, WorldWidth);
            int y = _random.Next(0, WorldHeight);

            if (_world.GetTile(x, y).Type != TileType.Wall) continue;
            if (usedPositions.Contains((x, y))) continue;
            
            _gems.Add(new GemRare(x, y));
            usedPositions.Add((x, y));
            gemCounter++;
        }
    }

    private void SpawnPlayer()
    {
        // Initialize player
        var startPosition = _world.GetStartPosition();
        _world.SetTile(startPosition.Item1, startPosition.Item2, TileType.Start);
        _player = new Player(startPosition.Item1, startPosition.Item2, ConsoleColor.DarkMagenta, '@');
        _player.Energy = DefaultPlayerEnergy;
        _player.Inventory = new Inventory();
        _player.IsExplored = true;
    }

    private void SpawnNPCs()
    {
        // Initialize NPCs
        for (int i = 0; i < 2; i++)
        {
            var npcSpawnPosition = _world.GetNpcSpawnPosition();
            var npc = new NPC(npcSpawnPosition.X, npcSpawnPosition.Y, ConsoleColor.Red, 'X');
            npc.IsExplored = false;
            _npcs.Add(npc);
        }
    }

    // Checks if player has won the game.
    // Conditions:
    // 1. The Relic has been picked up
    // 2. The player has returned back to start
    private bool CheckWinCondition()
    {
        // Picking up the relic
        if (_world.GetTile(_player.X, _player.Y).Type == TileType.Relic)
        {
            _hasFoundRelic = true;
            _world.SetTile(_player.X, _player.Y, TileType.Floor);
            _message = "> You found a relic! Return back to the start (+).";
        }

        return _world.GetTile(_player.X, _player.Y).Type == TileType.Start && _hasFoundRelic;
    }

    // Checks if player has lost the game.
    // Conditions:
    // 1. The player has run out of energy
    // 2. An NPC has stepped on player's position
    private bool CheckLoseCondition()
    {
        if (_player.Energy <= 0) return true;

        foreach (var npc in _npcs)
        {
            if (npc.X == _player.X && npc.Y == _player.Y)
            {
                return true;
            }
        }
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
        for (int i = 0; i < WallDestructionLuckOdds - 1; i++)
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