using System;

namespace Game
{
    // Enum to represent different entity types
    public enum EntityType
    {
        PlayerShip,
        EnemyShip,
        Location,
        Character
    }

    // Base class for all entities
    public abstract class Entity
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public EntityType Type { get; set; }

        public Entity(string name, Position position, EntityType type)
        {
            Name = name;
            Position = position;
            Type = type;
        }

        public abstract char[,] GetDisplayTile();
    }

    // Location class inheriting from Entity
    public class Location : Entity
    {
        public int Significance { get; set; }
        public List<Character> People { get; set; }
        public List<Item> LocationItems { get; set; }  

        public Location(string name, int significance, Position position)
            : base(name, position, EntityType.Location) 
        {
            Significance = significance;
            People = new List<Character>();
            LocationItems = new List<Item>();
        }

        public void AddPerson(Character person)
        {
            People.Add(person);
        }

        public void AddItemToLocation(Item item)
        {
            LocationItems.Add(item);
        }

        public void ShowPeople()
        {
            Console.WriteLine($"{Name}'s People:");
            foreach (var person in People)
            {
                Console.WriteLine($"- {person.Name}");
            }
        }

        public void ShowLocationItems()
        {
            Console.WriteLine($"{Name}'s Location Items:");
            foreach (var item in LocationItems)
            {
                Console.WriteLine($"- {item.Name}");
            }
        }

        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {' ', 'L'},
                {' ', ' '},
            };
        }
    }

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