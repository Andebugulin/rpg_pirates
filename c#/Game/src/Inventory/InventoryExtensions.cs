namespace Game
{
    // Extension methods for Character class
    public static class CharacterInventoryExtensions
    {
        public static Inventory InitializeInventory(this Character character)
        {
            return new Inventory(character);
        }

        public static bool EquipItem(this Character character, Item item)
        {
            if (item is Weapon weapon)
            {
                GameWorld.Instance.AddToCombatLog($"{character.Name} equipped {weapon.Name} (Damage: {weapon.Damage})");
                if (weapon.Type == WeaponType.Ranged)
                {
                    // Update ammunition if it's a ranged weapon
                    if (character.Ammunition <= 0)
                    {
                        GameWorld.Instance.AddToCombatLog($"{character.Name} has no ammunition for {weapon.Name}!");
                        return false;
                    }
                }
                return true;
            }
            else if (item is Relic relic)
            {
                GameWorld.Instance.AddToCombatLog($"{character.Name} equipped {relic.Name} (Power: {relic.Power})");
                return true;
            }

            return false;
        }
    }
}