using System;

namespace Game
{
    // Ship class inheriting from Entity
    public class Ship : Entity
    {
        public int Health { get; set; }
        public int AttackPower { get; set; }

        public List<Character> Crew { get; set; } 
        public List<Item> ShipItems { get; set; }

        public Ship(string name, int health, int attackPower, Position position)
            : base(name, position, EntityType.EnemyShip) 
        {
            Health = health;
            AttackPower = attackPower;
            Crew = new List<Character>();
            ShipItems = new List<Item>();
        }
         public void AddCrewMember(Character character)
        {
            Crew.Add(character);
        }

        public void AddItemToShip(Item item)
        {
            ShipItems.Add(item);
        }

        public void ShowCrew()
        {
            Console.WriteLine($"{Name}'s Crew:");
            foreach (var member in Crew)
            {
                Console.WriteLine($"- {member.Name}");
            }
        }

        public void ShowShipItems()
        {
            Console.WriteLine($"{Name}'s Ship Items:");
            foreach (var item in ShipItems)
            {
                Console.WriteLine($"- {item.Name}");
            }
        }

        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {' ', this.Name[0]},
                {' ', ' '},
            };
        }
    }
    
}