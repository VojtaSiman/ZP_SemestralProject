namespace zpsem;

public class GemRare(
    int x,
    int y,
    ConsoleColor color = ConsoleColor.Cyan,
    char glyph = '#',
    int value = 2500,
    string name = "Sapphire") : Gem(
    x,
    y,
    color,
    glyph,
    value,
    name);