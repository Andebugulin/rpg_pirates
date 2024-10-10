namespace Game 
{
    // Base class for all characters
    public abstract class Character : Entity
    {
        public int Health { get; set; }
        public int Strength { get; set; }
        public List<Item> Items { get; set; }

        public Character(string name, Position position, int health, int strength)
            : base(name, position, EntityType.Character) 
        {
            Health = health;
            Strength = strength;
            Items = new List<Item>();
        }
        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        public void ShowItems()
        {
            Console.WriteLine($"{Name}'s Items:");
            foreach (var item in Items)
            {
                Console.WriteLine($"- {item.Name}");
            }
        }

        public override char[,] GetDisplayTile()
        {
            return new char[,] { { 'C' } }; 
        }

        public abstract void DisplayInfo();
    }

    public class Civilian : Character
    {
        public Civilian(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Civilian {Name}: Health = {Health}, Strength = {Strength}");
        }
    }

    public class SpanishSoldier : Character
    {
        public SpanishSoldier(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Spanish Soldier {Name}: Health = {Health}, Strength = {Strength}");
        }
    }

    public class EnglishSoldier : Character
    {
        public EnglishSoldier(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"English Soldier {Name}: Health = {Health}, Strength = {Strength}");
        }
    }

    public class Pirate : Character
    {
        public Pirate(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Pirate {Name}: Health = {Health}, Strength = {Strength}");
        }
    }
}
