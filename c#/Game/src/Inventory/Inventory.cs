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
            Console.Clear();
            Console.WriteLine("Current Inventory:\n");
            
            
            if (_owner.Items.Count == 0)
            {
                Console.WriteLine("Your inventory is empty.");
            }
            else
            {
                for (int i = 0; i < _owner.Items.Count; i++)
                {
                    var item = _owner.Items[i];
                    item.DisplayInfo(); 
                }
            }
            
            Console.WriteLine("\nPress any key to return to combat...");
            Console.ReadKey(true);
        }

        public List<T> GetItemsOfType<T>() where T : Item
        {
            return _owner.Items.OfType<T>().ToList();
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
            // Test equipping items
            foreach (var item in character.Items)
            {
                CharacterInventoryExtensions.EquipItem(character, item);
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