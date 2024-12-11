using System;

namespace Game
{
    // Relic Class
    public class Relic : Item
    {
        public int Power { get; set; }
        public int Age { get; set; }

        public Relic(string name, ItemRarity rarity, int power, int age)
            : base(name, rarity)
        {
            Power = power;
            Age = age;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"{Rarity} {Name} (Power: {Power}, Age: {Age} years)");
        }
        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {'R'},
            };
        }
    }
}
