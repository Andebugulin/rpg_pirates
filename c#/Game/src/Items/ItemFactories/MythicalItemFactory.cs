using System;

namespace Game
{
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
