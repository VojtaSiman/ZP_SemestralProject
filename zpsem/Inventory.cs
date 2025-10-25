namespace zpsem;

public class Inventory
{
    public List<GemBasic> BasicGems { get; } = [];
    public List<GemRare> RareGems { get; } = [];


    public void AddGemToInventory(Gem gem)
    {
        if (gem.GetType() == typeof(GemRare))
        {
            RareGems.Add(gem as GemRare ?? throw new InvalidOperationException());
        }
        else if (gem.GetType() == typeof(GemBasic))
        {
            BasicGems.Add(gem as GemBasic ?? throw new InvalidOperationException());
        }
    }

    public string GetInventoryContent()
    {
        string output = "Inventory: {";

        if (BasicGems.Count > 0)
        {
            output += BasicGems[0].Name + " x" + BasicGems.Count;
        }

        if (BasicGems.Count > 0 && RareGems.Count > 0)
        {
            output += ", ";
        }
        
        if (RareGems.Count > 0)
        {
            output += RareGems[0].Name + " x" + RareGems.Count;
        }
        
        output += "}";
        return output;
    }
}