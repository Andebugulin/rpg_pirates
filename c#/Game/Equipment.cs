namespace Game
{
    // Enum to define equipment slot types
    public enum EquipmentSlotType
    {
        Weapon,
        Defensive,
        Utility
    }

    // Extension to Character class for equipment management
    public static class CharacterEquipmentExtensions
    {
        // Dictionary to store equipped items for each character
        private static readonly Dictionary<Character, Dictionary<EquipmentSlotType, Item>> _equippedItems 
            = new Dictionary<Character, Dictionary<EquipmentSlotType, Item>>();

        // Equip an item to a specific slot
        public static bool EquipItem(this Character character, Item item)
        {
            // Ensure character has an equipment slot dictionary
            if (!_equippedItems.ContainsKey(character))
            {
                _equippedItems[character] = new Dictionary<EquipmentSlotType, Item>();
            }

            var inventory = character.InitializeInventory();

            // Determine slot type based on item type
            EquipmentSlotType? slotType = item switch
            {
                Weapon => EquipmentSlotType.Weapon,
                Relic => EquipmentSlotType.Utility,
                // Add more mappings as needed
                _ => null
            };

            // Validate slot type and item
            if (!slotType.HasValue)
            {
                GameWorld.Instance.AddToCombatLog($"Cannot equip {item.Name}: Unrecognized item type");
                return false;
            }

            // Check if item is in inventory
            if (!inventory.Items.Contains(item))
            {
                GameWorld.Instance.AddToCombatLog($"{character.Name} does not have {item.Name} in inventory");
                return false;
            }

            // Unequip existing item in the slot if present
            if (_equippedItems[character].TryGetValue(slotType.Value, out Item existingItem))
            {
                UnequipItem(character, slotType.Value);
            }

            // Equip new item
            _equippedItems[character][slotType.Value] = item;
            inventory.RemoveItem(item);

            // Log equipment action
            GameWorld.Instance.AddToCombatLog($"{character.Name} equipped {item.Name} in {slotType.Value} slot");

            return true;
        }

        // Unequip an item from a specific slot
        public static bool UnequipItem(this Character character, EquipmentSlotType slotType)
        {
            if (!_equippedItems.ContainsKey(character) || 
                !_equippedItems[character].TryGetValue(slotType, out Item itemToUnequip))
            {
                GameWorld.Instance.AddToCombatLog($"No item equipped in {slotType} slot");
                return false;
            }

            var inventory = character.InitializeInventory();

            // Return item to inventory
            inventory.AddItem(itemToUnequip);
            _equippedItems[character].Remove(slotType);

            GameWorld.Instance.AddToCombatLog($"{character.Name} unequipped {itemToUnequip.Name} from {slotType} slot");
            return true;
        }

        // Get currently equipped item in a specific slot
        public static Item GetEquippedItem(this Character character, EquipmentSlotType slotType)
        {
            if (_equippedItems.TryGetValue(character, out var slots) && 
                slots.TryGetValue(slotType, out Item equippedItem))
            {
                return equippedItem;
            }
            return null;
        }

        // Display equipped items
        public static void DisplayEquipment(this Character character)
        {
            GameWorld.Instance.AddToCombatLog($"\n=== {character.Name}'s Equipment ===");
            
            foreach (EquipmentSlotType slotType in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                var equippedItem = GetEquippedItem(character, slotType);
                if (equippedItem != null)
                {
                    GameWorld.Instance.AddToCombatLog($"{slotType} Slot: {equippedItem.Name}");
                }
                else
                {
                    GameWorld.Instance.AddToCombatLog($"{slotType} Slot: Empty");
                }
            }
        }
        public static void ShowEquipmentMenu(this Character character)
        {
            bool inEquipmentMenu = true;
            var inventory = character.InitializeInventory();

            while (inEquipmentMenu)
            {
                // Clear and display current equipment
                Console.Clear();
                character.DisplayEquipment();
                Console.WriteLine("\nEquipment Menu:");
                Console.WriteLine("1: Equip Weapon");
                Console.WriteLine("2: Equip Defensive Item");
                Console.WriteLine("3: Equip Utility Item");
                Console.WriteLine("4: Unequip Weapon");
                Console.WriteLine("5: Unequip Defensive Item");
                Console.WriteLine("6: Unequip Utility Item");
                Console.WriteLine("Q: Exit Menu");

                char choice = Console.ReadKey(true).KeyChar;
                switch (char.ToUpper(choice))
                {
                    case '1':
                        EquipItemByType<Weapon>(character, EquipmentSlotType.Weapon);
                        break;
                    case '2':
                        EquipItemByType<Weapon>(character, EquipmentSlotType.Defensive);
                        break;
                    case '3':
                        EquipItemByType<Relic>(character, EquipmentSlotType.Utility);
                        break;
                    case '4':
                        character.UnequipItem(EquipmentSlotType.Weapon);
                        break;
                    case '5':
                        character.UnequipItem(EquipmentSlotType.Defensive);
                        break;
                    case '6':
                        character.UnequipItem(EquipmentSlotType.Utility);
                        break;
                    case 'Q':
                        inEquipmentMenu = false;
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }

        // Helper method to equip items of specific type
        private static void EquipItemByType<T>(Character character, EquipmentSlotType slotType) 
            where T : Item
        {
            var inventory = character.InitializeInventory();
            var compatibleItems = inventory.GetItemsOfType<T>();

            if (compatibleItems.Count == 0)
            {
                GameWorld.Instance.AddToCombatLog($"No compatible items for {slotType} slot.");
                return;
            }

            Console.WriteLine($"\nAvailable {slotType} Items:");
            for (int i = 0; i < compatibleItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {compatibleItems[i].Name}");
            }
            Console.WriteLine("Select an item (or press Q to cancel)");

            char choice = Console.ReadKey(true).KeyChar;
            if (char.ToUpper(choice) == 'Q') return;

            if (int.TryParse(choice.ToString(), out int index) && 
                index > 0 && index <= compatibleItems.Count)
            {
                CharacterEquipmentExtensions.EquipItem(character, compatibleItems[index - 1]);
            }
        }
    }
}