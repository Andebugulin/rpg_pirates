using System;
using System.Collections.Generic;

namespace Game
{
    // Class representing each cell on the map
    public class Cell
    {
        public List<Entity> Entities { get; set; }

        public Cell()
        {
            Entities = new List<Entity>();
        }

        // Gets the display tile for this cell
        public char[,] GetDisplayTile()
        {
            if (Entities.Count == 0)
            {
                // If empty, return a NxN water tile
                return new char[,]
                {
                    {'~'},
                   
                };
            }

            // Priority: PlayerShip > EnemyShip > Location
            if (Entities.Exists(e => e.Type == EntityType.PlayerShip))
                return Entities.Find(e => e.Type == EntityType.PlayerShip).GetDisplayTile();
            if (Entities.Exists(e => e.Type == EntityType.EnemyShip))
                return Entities.Find(e => e.Type == EntityType.EnemyShip).GetDisplayTile();
            if (Entities.Exists(e => e.Type == EntityType.Location))
                return Entities.Find(e => e.Type == EntityType.Location).GetDisplayTile();

            // Default water tile
            return new char[,]
            {
                    {'~'},
                   
                };
        }
    }

    public class GameWorld
    {
        private int width;
        private int height;
        private Random rng;

        private List<Entity> entities; 
        public PlayerShip playerShip; // Main player ship

        private static GameWorld _instance; // Singleton instance

        private Cell[,] grid; // Grid of cells

        private GameWorld()
        {
            width = 40; 
            height = 15; 
            rng = new Random();

            TimeOfDay = "Day";
            Weather = "Clear";

            // Initialize grid
            grid = new Cell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    grid[x, y] = new Cell();

            entities = new List<Entity>();

            // Initialize player ship
            playerShip = new PlayerShip("Black Pearl", Utils.GetRandomPosition(width, height));
            AddEntity(playerShip);

            // Initialize enemy ships
            AddEntity(new Ship("Spanish Galleon", Utils.GetRandomPosition(width, height)));
            AddEntity(new Ship("French Frigate", Utils.GetRandomPosition(width, height)));
            AddEntity(new Ship("English Man-O-War", Utils.GetRandomPosition(width, height)));

            // Initialize locations
            AddEntity(new Location("Port Royal", Utils.GetRandomPosition(width, height)));
            AddEntity(new Location("Tortuga", Utils.GetRandomPosition(width, height)));
        }

        public static GameWorld Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameWorld();
                }
                return _instance;
            }
        }

        public string TimeOfDay { get; set; }
        public string Weather { get; set; }

        // Adds an entity to the game world and places it on the grid
        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
            if (IsWithinBounds(entity.Position))
            {
                grid[entity.Position.X, entity.Position.Y].Entities.Add(entity);
            }
            else
            {
                Console.WriteLine($"Entity {entity.Name} position out of bounds.");
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
                for (int row = 0; row < 1; row++) // Each NxN cell has N rows
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Get the NxN tile for the current cell
                        char[,] tile = grid[x, y].GetDisplayTile();
                        for (int col = 0; col < 1; col++) // Each NxN cell has N columns
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
    }
}
