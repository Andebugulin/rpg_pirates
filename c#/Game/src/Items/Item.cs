using System;

namespace Game
{
    // Enum for Rarity of Items
    public enum ItemRarity
    {
        Common,
        Rare,
        Mythical
    }

    // Base class for all Items
    public abstract class Item
    {
        public string Name { get; set; }
        public ItemRarity Rarity { get; set; }
        public Position Position { get; set; }

        public Item(string name, ItemRarity rarity)
        {
            Name = name;
            Rarity = rarity;
        }

        public abstract void DisplayInfo();
        public abstract char[,] GetDisplayTile();
    }
}
