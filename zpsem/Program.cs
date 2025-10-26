namespace zpsem;

static class Program
{
    static void Main()
    {
        Console.Title = "Rogue Relic by Vojta Siman";
        Console.CursorVisible = false;
        
        var game = new Game();
        game.Run();
    }
}