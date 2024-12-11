using System;

namespace Game
{
    // TreasureMap Class
    public class TreasureMap : Item
    {
        public int LocationComplexity { get; set; }
        public string MapCondition { get; set; }

        public TreasureMap(string name, ItemRarity rarity, int locationComplexity, string mapCondition)
            : base(name, rarity)
        {
            LocationComplexity = locationComplexity;
            MapCondition = mapCondition;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"{Rarity} {Name} (Complexity: {LocationComplexity}, Condition: {MapCondition})");
        }
        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {'M'},
            };
        }
    }
}
