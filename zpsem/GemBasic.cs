namespace zpsem;

public class GemBasic(
    int x,
    int y,
    ConsoleColor color = ConsoleColor.Blue,
    char glyph = '#',
    int value = 1000,
    string name = "Opal") : Gem(
    x,
    y,
    color,
    glyph,
    value,
    name);