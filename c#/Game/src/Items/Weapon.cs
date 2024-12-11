using System;

namespace Game
{
    // Enum for Weapon Type
    public enum WeaponType
    {
        Melee,
        Ranged
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
}
