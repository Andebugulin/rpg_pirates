namespace Game
{
    public class Inventory
    {
        private readonly Character _owner;
        private const int MaxCapacity = 20;

        public IReadOnlyList<Item> Items => _owner.Items.AsReadOnly();
        public int Count => _owner.Items.Count;
        public bool IsFull => Count >= MaxCapacity;

        public Inventory(Character owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public bool AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (IsFull)
            {
                GameWorld.Instance.AddToCombatLog($"{_owner.Name}'s inventory is full!");
                return false;
            }

            _owner.AddItem(item);
            GameWorld.Instance.AddToCombatLog($"{_owner.Name} obtained {item.Name}.");
            
            // Add appropriate strategy based on item type
            if (item is Weapon weapon)
            {
                if (weapon.Type == WeaponType.Melee)
                    _owner.AddStrategy(new MeleeAction());
                else if (weapon.Type == WeaponType.Ranged)
                    _owner.AddStrategy(new RangedAction());
            }
            else if (item is Relic)
            {
                _owner.AddStrategy(new MagicAction());
            }

            return true;
        }

        public bool RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool removed = _owner.Items.Remove(item);
            if (removed)
                GameWorld.Instance.AddToCombatLog($"{_owner.Name} removed {item.Name} from inventory.");
            
            return removed;
        }

        public bool RemoveItemByName(string itemName)
        {
            var item = _owner.Items.FirstOrDefault(i => 
                i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            return item != null && RemoveItem(item);
        }

        public void DisplayInventory()
        {
            GameWorld.Instance.AddToCombatLog($"\n=== {_owner.Name}'s Inventory ===");
            GameWorld.Instance.AddToCombatLog($"Items: {Count}/{MaxCapacity}");
            
            if (_owner.Items.Count == 0)
            {
                GameWorld.Instance.AddToCombatLog("Inventory is empty.");
                return;
            }

            // Group items by type for better organization
            var groupedItems = _owner.Items.GroupBy(item => item.GetType().Name);
            
            foreach (var group in groupedItems)
            {
                GameWorld.Instance.AddToCombatLog($"\n{group.Key}s:");
                foreach (var item in group)
                {
                    item.DisplayInfo();
                }
            }
        }

        public List<T> GetItemsOfType<T>() where T : Item
        {
            return _owner.Items.OfType<T>().ToList();
        }
    }

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

    // Example usage class
    public class InventoryExample
    {
        public static void TestInventorySystem()
        {
            // Create a character (using one of your concrete classes)
            var character = new Pirate("Jack", new Position(0, 0), 100, 10);
            var inventory = character.InitializeInventory();

            // Create some items using your existing factories
            var commonFactory = new CommonItemFactory();
            var rareFactory = new RareItemFactory();
            var mythicalFactory = new MythicalItemFactory();

            // Add items to inventory
            inventory.AddItem(commonFactory.CreateWeapon());
            inventory.AddItem(rareFactory.CreateWeapon());
            inventory.AddItem(mythicalFactory.CreateRelic());

            // Display inventory
            inventory.DisplayInventory();

            // Test equipping items
            foreach (var item in character.Items)
            {
                character.EquipItem(item);
            }

            // Test removing an item
            inventory.RemoveItemByName("Rusty Cutlass");

            // Display updated inventory
            inventory.DisplayInventory();

            // Get all weapons
            var weapons = inventory.GetItemsOfType<Weapon>();
            GameWorld.Instance.AddToCombatLog($"\nWeapons in inventory: {weapons.Count}");
        }
    }
}