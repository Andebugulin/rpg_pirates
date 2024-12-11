using System;

namespace Game
{
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
}
