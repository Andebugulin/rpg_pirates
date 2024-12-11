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
}