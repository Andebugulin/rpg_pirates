using System;

class Program
{
    static void Main(string[] args)
    {
        // singleton test
        var gameWorld = GameWorld.Instance;
        gameWorld.DisplayWorldInfo();
    }
}
