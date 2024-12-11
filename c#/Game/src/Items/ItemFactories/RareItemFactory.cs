using System;

namespace Game
{
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
}
