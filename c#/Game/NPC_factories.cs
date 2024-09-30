namespace Game
{
// Factory for Ships
public class ShipFactory
{
    public static Ship CreateShip(string type, string name, Position position)
    {
        switch (type)
        {
            case "Galleon":
                return new Ship(name, 200, 50, position);
            case "Frigate":
                return new Ship(name, 150, 60, position);
            case "ManO'War":
                return new Ship(name, 250, 70, position);
            default:
                throw new ArgumentException("Unknown ship type.");
        }
    }
}

// Factory for Locations
public class LocationFactory
{
    public static Location CreateLocation(string type, string name, Position position)
    {
        switch (type)
        {
            case "Port":
                return new Location(name, 100, position);
            case "Island":
                return new Location(name, 50, position); 
            default:
                throw new ArgumentException("Unknown location type.");
        }
    }
}

// Character Factory 
 public class CharacterFactory
    {
        public static Character CreateCharacter(string type, string name, Position position)
        {
            switch (type)
            {
                case "Pirate":
                    return new Pirate(name, position, 100, 50);
                case "EnglishSoldier":
                    return new EnglishSoldier(name, position, 150, 40);
                case "SpanishSoldier":
                    return new SpanishSoldier(name, position, 160, 45);
                case "Civilian":
                    return new Civilian(name, position, 80, 10);
                default:
                    throw new ArgumentException("Unknown character type.");
            }
        }
    }

}