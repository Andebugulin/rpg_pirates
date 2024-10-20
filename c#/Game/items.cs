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

    // Enum for Weapon Type
    public enum WeaponType
    {
        Melee,
        Ranged
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

    // Weapon Class
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public WeaponType Type { get; set; }

        public Weapon(string name, ItemRarity rarity, int damage, WeaponType type)
            : base(name, rarity)
        {
            Damage = damage;
            Type = type;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"{Rarity} {Name} (Type: {Type}, Damage: {Damage})");
        }
        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {'W'},
            };
        }
    }

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

    // Abstract ItemFactory
    public abstract class ItemFactory
    {
        public abstract Weapon CreateWeapon();
        public abstract TreasureMap CreateTreasureMap();
        public abstract Relic CreateRelic();
    }

    // Common Item Factory
    public class CommonItemFactory : ItemFactory
    {
        public override Weapon CreateWeapon()
        {
            return new Weapon("Rusty Cutlass", ItemRarity.Common, 10, WeaponType.Melee);
        }

        public override TreasureMap CreateTreasureMap()
        {
            return new TreasureMap("Old Map", ItemRarity.Common, 2, "Torn");
        }

        public override Relic CreateRelic()
        {
            return new Relic("bracelet", ItemRarity.Common, 5, 100);
        }
    }

    // Rare Item Factory
    public class RareItemFactory : ItemFactory
    {
        public override Weapon CreateWeapon()
        {
            return new Weapon("Sharp Cutlass", ItemRarity.Rare, 25, WeaponType.Melee);
        }

        public override TreasureMap CreateTreasureMap()
        {
            return new TreasureMap("Intricate Map", ItemRarity.Rare, 5, "Well-Preserved");
        }

        public override Relic CreateRelic()
        {
            return new Relic("Golden ring", ItemRarity.Rare, 20, 300);
        }
    }

    // Mythical Item Factory
    public class MythicalItemFactory : ItemFactory
    {
        public override Weapon CreateWeapon()
        {
            return new Weapon("Balanced fancy Cutlass", ItemRarity.Mythical, 50, WeaponType.Melee);
        }

        public override TreasureMap CreateTreasureMap()
        {
            return new TreasureMap("Ancient Map", ItemRarity.Mythical, 10, "Pristine");
        }

        public override Relic CreateRelic()
        {
            return new Relic("Compass that doesn't point north", ItemRarity.Mythical, 50, 1000);
        }
    }
}
