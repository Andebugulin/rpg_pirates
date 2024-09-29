namespace Game
{
     public class PlayerShip : Entity
    {
        public override EntityType Type => EntityType.PlayerShip;

        // A NxN tile representation of the player ship
        public override char[,] GetDisplayTile()
        {
            return new char[,]
                {
                    {'P'},
                   
                };
        }

        public PlayerShip(string name, Position position)
        {
            Name = name;
            Position = position;
        }
    }

    public class Location : Entity
    {
        public override EntityType Type => EntityType.Location;

        // A NxN tile representation of a location
        public override char[,] GetDisplayTile()
        {
            return new char[,]
             {
                    {'L'},
                   
                };
        }

        public Location(string name, Position position)
        {
            Name = name;
            Position = position;
        }
    }

    public class Ship : Entity
    {
        public override EntityType Type => EntityType.EnemyShip;

        // A NxN tile representation of an enemy ship
        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                    {'E'},
                   
                };
        }

        public Ship(string name, Position position)
        {
            Name = name;
            Position = position;
        }
    }
}