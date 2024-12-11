using System;

namespace Game
{
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
}