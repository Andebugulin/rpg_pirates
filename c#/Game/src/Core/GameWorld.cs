using System;
using System.Collections.Generic;

namespace Game
{
    // Class representing each cell on the map
    public class Cell
    {
        public List<Entity> Entities { get; set; }
        public List<Item> Items { get; set; }

        public Cell()
        {
            Entities = new List<Entity>();
            Items = new List<Item>();
        }

        // Gets the display tile for this cell
        public char[,] GetDisplayTile()
        {
            if (Entities.Count == 0 && Items.Count == 0)
            {
                // If empty, return a NxN water tile
                return new char[,]
                {
                    {' ', '~'},
                    {' ', ' '},
                };
            }

            // Priority: PlayerShip > EnemyShip > Location
            if (Entities.Exists(e => e.Type == EntityType.PlayerShip))
                return Entities.Find(e => e.Type == EntityType.PlayerShip).GetDisplayTile();
            if (Entities.Exists(e => e.Type == EntityType.EnemyShip))
                return Entities.Find(e => e.Type == EntityType.EnemyShip).GetDisplayTile();
            if (Entities.Exists(e => e.Type == EntityType.Location))
                return Entities.Find(e => e.Type == EntityType.Location).GetDisplayTile();

            if (Items.Count > 0)
                return Items[0].GetDisplayTile();
                
            // Default water tile
            return new char[,]
            {
                    {' ', '~'},
                    {' ', ' '},
                   
                };
        }
    }

    public class GameWorld
    {
        public int width;
        public int height;
        public List<Entity> entities; 
        public Ship playerShip;
        public QuestManager questManager;

        private static GameWorld _instance; // Singleton instance

        private Cell[,] grid; // Grid of cells
        public Queue<string> combatLog = new Queue<string>();
        public const int MAX_LOG_ENTRIES = 5;

        private GameWorld()
    {
        width = 40;
        height = 15;
        TimeOfDay = "Day";
        Weather = "Clear";
        entities = new List<Entity>();
        questManager = new QuestManager();
        
        // Initialize grid
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new Cell();
    }

    // Separate method for world initialization
    private void InitializeWorld()
    {
        // Create item factories first
        CommonItemFactory commonFactory = new CommonItemFactory();
        RareItemFactory rareFactory = new RareItemFactory();
        MythicalItemFactory mythicalFactory = new MythicalItemFactory();

        // Create all locations first
        Position portRoyalPosition = new Position(10, 5);
        Position tortugaPosition = new Position(15, 9);
        Location portRoyal = LocationFactory.CreateLocation("Port", "Port Royal", portRoyalPosition);
        Location tortuga = LocationFactory.CreateLocation("Island", "Tortuga", tortugaPosition);

        // Create all ships first
        Position blackPearlPosition = new Position(20, 8);
        Position dutchmanPosition = new Position(25, 12);
        
        Ship blackPearl = ShipFactory.CreateShip("Galleon", "Black Pearl", blackPearlPosition);
        Ship flyingDutchman = ShipFactory.CreateShip("ManO'War", "Flying Dutchman", dutchmanPosition);

        // Create all characters
        Position crewPosition1 = new Position(20, 8);
        Position crewPosition2 = new Position(21, 9);
        
        Character merchantJohn = CharacterFactory.CreateCharacter("Civilian", "Merchant John", Utils.GetRandomPosition(width, height));
        Character soldierJames = CharacterFactory.CreateCharacter("EnglishSoldier", "Soldier James", Utils.GetRandomPosition(width, height));
        Character pirateJack = CharacterFactory.CreateCharacter("Pirate", "Pirate Jack", Utils.GetRandomPosition(width, height));
        Character innkeeperMaria = CharacterFactory.CreateCharacter("Civilian", "Innkeeper Maria", Utils.GetRandomPosition(width, height));
        Character captainJack = CharacterFactory.CreateCharacter("Pirate", "Captain Jack Sparrow", crewPosition1);
        Character crewMember1 = CharacterFactory.CreateCharacter("Pirate", "Crewman Gibbs", crewPosition2);
        Character davyJones = CharacterFactory.CreateCharacter("Pirate", "Davy Jones", Utils.GetRandomPosition(width, height));
        Character bootstrapBill = CharacterFactory.CreateCharacter("Pirate", "Bootstrap Bill", Utils.GetRandomPosition(width, height));
        Character billTurner = CharacterFactory.CreateCharacter("Pirate", "Bill Turner", Utils.GetRandomPosition(width, height));
        Character maccus = CharacterFactory.CreateCharacter("Pirate", "Maccus", Utils.GetRandomPosition(width, height));
        Character clanker = CharacterFactory.CreateCharacter("Pirate", "Clanker", Utils.GetRandomPosition(width, height));
        Character victor = CharacterFactory.CreateCharacter("Pirate", "Victor", Utils.GetRandomPosition(width, height));

        // Add characters to locations
        portRoyal.AddPerson(merchantJohn);
        portRoyal.AddPerson(soldierJames);
        tortuga.AddPerson(pirateJack);
        tortuga.AddPerson(innkeeperMaria);

        // Add items to locations
        portRoyal.AddItemToLocation(commonFactory.CreateWeapon());
        portRoyal.AddItemToLocation(rareFactory.CreateRelic());
        tortuga.AddItemToLocation(mythicalFactory.CreateTreasureMap());
        tortuga.AddItemToLocation(commonFactory.CreateRelic());

        // Add crew to ships
        blackPearl.AddCrewMember(captainJack);
        blackPearl.AddCrewMember(crewMember1);
        flyingDutchman.AddCrewMember(davyJones);
        flyingDutchman.AddCrewMember(bootstrapBill);
        flyingDutchman.AddCrewMember(billTurner);
        flyingDutchman.AddCrewMember(maccus);
        flyingDutchman.AddCrewMember(clanker);
        flyingDutchman.AddCrewMember(victor);

        // Add items to ships
        blackPearl.AddItemToShip(rareFactory.CreateWeapon());
        blackPearl.AddItemToShip(mythicalFactory.CreateRelic());
        flyingDutchman.AddItemToShip(mythicalFactory.CreateWeapon());
        flyingDutchman.AddItemToShip(rareFactory.CreateTreasureMap());

        // Set player ship
        playerShip = blackPearl;

        // Add entities to world
        AddEntity(playerShip);
        AddEntity(flyingDutchman);
        AddEntity(tortuga);
        AddEntity(portRoyal);

        // Display information
        Console.WriteLine("Locations:");
        portRoyal.ShowPeople();
        portRoyal.ShowLocationItems();
        tortuga.ShowPeople();
        tortuga.ShowLocationItems();

        Console.WriteLine("\nShips:");
        blackPearl.ShowCrew();
        blackPearl.ShowShipItems();
        flyingDutchman.ShowCrew();
        flyingDutchman.ShowShipItems();

        // Create and set up quests AFTER everything else is initialized
        var quest = QuestFactory.CreatePirateHuntQuest(3);
        var pirate_for_quest = captainJack; 
        // Now we can safely register observers and add quests
        questManager.RegisterObserverForQuest(pirate_for_quest, quest);
        questManager.AddQuest(quest);
    }
        public static GameWorld Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameWorld();
                    _instance.InitializeWorld();
                }
                return _instance;
            }
        }

        public void ChangePlayerShip(Ship newShip)
        {
            playerShip = newShip;
        }

        public string TimeOfDay { get; set; }
        public string Weather { get; set; }

        public bool CheckShipCollision(Ship ship1, Ship ship2)
        {
            return ship1.Position.X == ship2.Position.X && ship1.Position.Y == ship2.Position.Y;
        }

        public void InitiateCrewCombat(Ship ship1, Ship ship2)
        {
            CrewCombatScene combatScene = new CrewCombatScene(ship1, ship2);
            combatScene.StartCombat();
        }
        public void InitiateLocationScene(Location location, Ship ship)
        {
            LocationScene locationScene = new LocationScene(location, ship);
            locationScene.Start();
        }

        // Adds an entity to the game world and places it on the grid
        public void AddEntity(Entity entity)
        {
            if (IsWithinBounds(entity.Position))
            {
                entities.Add(entity);
                grid[entity.Position.X, entity.Position.Y].Entities.Add(entity);
            }
            else
            {
                Console.WriteLine($"Entity {entity.Name} position out of bounds.");
                entity.Position = Utils.GetRandomPosition(width, height); // Reposition entity within bounds
                AddEntity(entity); // Re-add with valid position
            }
        }

        // Checks if the position is within the grid bounds
        private bool IsWithinBounds(Position pos)
        {
            return pos.X >= 0 && pos.X < width && pos.Y >= 0 && pos.Y < height;
        }

        // Display the map, drawing each cell as a NxN grid
        public void DisplayMap()
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                // For each cell row, print 3 rows of NxN tiles
                for (int row = 0; row < 2; row++) // Each NxN cell has N rows
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Get the NxN tile for the current cell
                        char[,] tile = grid[x, y].GetDisplayTile();
                        for (int col = 0; col < 2; col++) // Each NxN cell has N columns
                        {
                            Console.Write(tile[row, col]);
                        }
                    }
                    Console.WriteLine(); // Move to the next line after printing a row of the NxN grid
                }
            }
        }

        public void DisplayWorldInfo()
        {
            Console.WriteLine($"Map Size: {width} x {height}");
            Console.WriteLine($"Time of Day: {TimeOfDay}");
            Console.WriteLine($"Weather: {Weather}");
            Console.WriteLine($"Player Ship Position: X = {playerShip.Position.X}, Y = {playerShip.Position.Y}");
            Console.WriteLine("Entities:");
            foreach (var entity in entities)
            {
                Console.WriteLine($"{entity.Name} ({entity.Type}) at X = {entity.Position.X}, Y = {entity.Position.Y}");
            }
        }

        // Method to move a ship 
        public void MoveEntity(Entity entity, Position newPosition)
        {
            if (!IsWithinBounds(newPosition))
            {
                Console.WriteLine("Move out of bounds!");
                return;
            }

            // Remove from old cell
            grid[entity.Position.X, entity.Position.Y].Entities.Remove(entity);

            // Update position
            entity.Position = newPosition;

            // Add to new cell
            grid[newPosition.X, newPosition.Y].Entities.Add(entity);
        }
        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            grid[entity.Position.X, entity.Position.Y].Entities.Remove(entity);
            
            // If it's a character in a ship's crew, remove it from there too
            foreach (var ship in entities.OfType<Ship>())
            {
                ship.Crew.Remove(entity as Character);
            }
        }

        public void AddToCombatLog(string message)
        {
            combatLog.Enqueue(message);
            if (combatLog.Count > MAX_LOG_ENTRIES)
            {
                combatLog.Dequeue();
            }
        }
    }
}
