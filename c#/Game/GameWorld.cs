using System;
using System.Collections.Generic;

public class GameWorld
{
    // static instance (singleton pattern)
    private static GameWorld _instance;

    // private constructor
    private GameWorld()
    {
        // initialize world map and NPCs
        WorldMap = new List<string>() { "Forest", "Village", "Dungeon" };
        NPCs = new List<NPC>() {
            new NPC("Guard", "Protector"),
            new NPC("Merchant", "Trader"),
        };
        TimeOfDay = "Day";
        Weather = "Clear";
    }

    // get the instance (singleton pattern)
    public static GameWorld Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameWorld();
            }
            return _instance;
        }
    }

    public List<string> WorldMap { get; private set; }

    public List<NPC> NPCs { get; private set; }

    public string TimeOfDay { get; set; }
    public string Weather { get; set; }

    public void DisplayWorldInfo()
    {
        Console.WriteLine("World Map: " + string.Join(", ", WorldMap));
        Console.WriteLine("NPCs:");
        foreach (var npc in NPCs)
        {
            Console.WriteLine($"- {npc.Name} ({npc.Role})");
        }
        Console.WriteLine($"Time of Day: {TimeOfDay}");
        Console.WriteLine($"Weather: {Weather}");
    }
}

public class NPC
{
    public string Name { get; private set; }
    public string Role { get; private set; }

    public NPC(string name, string role)
    {
        Name = name;
        Role = role;
    }
}
