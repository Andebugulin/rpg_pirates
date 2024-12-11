using System;

namespace Game
{
    // Abstract ItemFactory
    public abstract class ItemFactory
    {
        public abstract Weapon CreateWeapon();
        public abstract TreasureMap CreateTreasureMap();
        public abstract Relic CreateRelic();
    }
}
