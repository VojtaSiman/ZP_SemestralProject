namespace zpsem;

static class Program
{
    static void Main()
    {
        Console.Title = "Rogue Relic by Vojta Siman";
        Console.CursorVisible = false;
        
        bool isGameRunning = true;

        while (isGameRunning)
        {
            var wishToQuitGame = PlayGame();
            if (wishToQuitGame) break;
            
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
            {
                isGameRunning = false;
            }
        }
    }

    static bool PlayGame()
    {
        var game = new Game();
        var wishToQuitGame = game.Run();
        return wishToQuitGame;
    }
}