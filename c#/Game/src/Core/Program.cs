using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Game.Database;

namespace Game
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Setup Dependency Injection and Database
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Ensure Database is created and seeded
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            Console.Clear();
            GameWorld game = GameWorld.Instance;
            bool isRunning = true;
            
            // Define AI ships as all that are not player ship
            List<ShipAI> shipAIs = game.entities
                .OfType<Ship>()
                .Where(s => s != game.playerShip)
                .Select(s => new ShipAI(s))
                .ToList();
            
            // Time required to read instructions
            Thread.Sleep(5000);

            while (isRunning)
            {
                game.DisplayMap();
                Console.WriteLine("Move the player ship (HJLK) or press Q to quit:");
                
                foreach (var ai in shipAIs)
                {
                    ai.UpdateBehavior(game);
                }

                char input = Console.ReadKey().KeyChar;
                Position newPosition = game.playerShip.Position;

                switch (char.ToUpper(input))
                {
                    case 'K': newPosition = new Position(game.playerShip.Position.X, game.playerShip.Position.Y - 1); break;
                    case 'J': newPosition = new Position(game.playerShip.Position.X, game.playerShip.Position.Y + 1); break;
                    case 'H': newPosition = new Position(game.playerShip.Position.X - 1, game.playerShip.Position.Y); break;
                    case 'L': newPosition = new Position(game.playerShip.Position.X + 1, game.playerShip.Position.Y); break;
                    case 'Q': isRunning = false; continue;
                }

                Ship collidedShip = game.entities
                    .OfType<Ship>()
                    .FirstOrDefault(s => s != game.playerShip && 
                                        s.Position.X == newPosition.X && 
                                        s.Position.Y == newPosition.Y);

                Location collidedLocation = game.entities
                    .OfType<Location>()
                    .FirstOrDefault(l => l.Position.X == newPosition.X && 
                                        l.Position.Y == newPosition.Y);
                    
                if (collidedShip != null)
                {
                    game.InitiateCrewCombat(game.playerShip, collidedShip);
                }
                else if (collidedLocation != null)
                {
                    game.InitiateLocationScene(collidedLocation, game.playerShip);
                }
                else
                {
                    game.MoveEntity(game.playerShip, newPosition);
                }
                Console.Clear();
            }

            Console.WriteLine("Thanks for playing!");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configure DbContext
            services.AddDbContext<GameDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            services.AddScoped<GameRepository>();
        }
    }
}