using System;

namespace Game
{

    // Enum to represent different entity types
    public enum EntityType
    {
        None,
        PlayerShip,
        EnemyShip,
        Location
    }

    // Base class for all entities
    public abstract class Entity
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public abstract EntityType Type { get; }
        public abstract char[,] GetDisplayTile();
    }
}