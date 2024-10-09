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
        public Item[] items;

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

        public Location(string name, int significance, Position position)
            : base(name, position, EntityType.Location) 
        {
            Significance = significance;
            items = new Item[7];
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

        public Ship(string name, int health, int attackPower, Position position)
            : base(name, position, EntityType.EnemyShip) 
        {
            Health = health;
            AttackPower = attackPower;
            items = new Item[5];
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