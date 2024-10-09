using System;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the game world (Singleton pattern)
            GameWorld game = GameWorld.Instance;

            // Continuously display the map and allow player to interact
            bool isRunning = true;

            while (isRunning)
            {
                game.DisplayMap();


                Console.WriteLine("Move the player ship (WASD) or press Q to quit:");

                // Get user input for movement
                char input = Console.ReadKey().KeyChar;

                // Handle input
                switch (char.ToUpper(input))
                {
                    case 'K': // Move up
                        game.MoveEntity(game.playerShip, new Position(game.playerShip.Position.X, game.playerShip.Position.Y - 1));
                        break;
                    case 'J': // Move down
                        game.MoveEntity(game.playerShip, new Position(game.playerShip.Position.X, game.playerShip.Position.Y + 1));
                        break;
                    case 'H': // Move left
                        game.MoveEntity(game.playerShip, new Position(game.playerShip.Position.X - 1, game.playerShip.Position.Y));
                        break;
                    case 'L': // Move right
                        game.MoveEntity(game.playerShip, new Position(game.playerShip.Position.X + 1, game.playerShip.Position.Y));
                        break;
                    case 'Q': // Quit the game
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input, use W/A/S/D to move or Q to quit.");
                        break;
                }

                // Clear the screen for the next update
                Console.Clear();
            }

            Console.WriteLine("Thanks for playing!");
        }
    }
}
