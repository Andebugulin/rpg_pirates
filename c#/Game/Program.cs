using System;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWorld game = GameWorld.Instance;
            bool isRunning = true;

            while (isRunning)
            {
                game.DisplayMap();
                Console.WriteLine("Move the player ship (WASD) or press Q to quit:");

                char input = Console.ReadKey().KeyChar;
                Position newPosition = game.playerShip.Position;

                switch (char.ToUpper(input))
                {
                    case 'K':
                        newPosition = new Position(game.playerShip.Position.X, game.playerShip.Position.Y - 1);
                        break;
                    case 'J':
                        newPosition = new Position(game.playerShip.Position.X, game.playerShip.Position.Y + 1);
                        break;
                    case 'H':
                        newPosition = new Position(game.playerShip.Position.X - 1, game.playerShip.Position.Y);
                        break;
                    case 'L':
                        newPosition = new Position(game.playerShip.Position.X + 1, game.playerShip.Position.Y);
                        break;
                    case 'Q':
                        isRunning = false;
                        continue;
                }

                // Check for potential collisions before moving
                Ship collidedShip = game.entities
                    .OfType<Ship>()
                    .FirstOrDefault(s => s != game.playerShip && 
                                        s.Position.X == newPosition.X && 
                                        s.Position.Y == newPosition.Y);

                if (collidedShip != null)
                {
                    game.InitiateCrewCombat(game.playerShip, collidedShip);
                }
                else
                {
                    game.MoveEntity(game.playerShip, newPosition);
                }

                Console.Clear();
            }

        Console.WriteLine("Thanks for playing!");
        }
    }
}
