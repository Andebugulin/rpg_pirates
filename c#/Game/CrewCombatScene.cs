namespace Game
{
    public class CrewCombatScene
    {
        private Ship ship1;
        private Ship ship2;
        private const int COMBAT_GRID_WIDTH = 15;
        private const int COMBAT_GRID_HEIGHT = 10;
        private Cell[,] combatGrid;
        private Character playerCharacter;

        public CrewCombatScene(Ship ship1, Ship ship2)
        {
            this.ship1 = ship1;
            this.ship2 = ship2;
            this.combatGrid = new Cell[COMBAT_GRID_WIDTH, COMBAT_GRID_HEIGHT];
            InitializeCombatGrid();
        }

        private void InitializeCombatGrid()
        {
            // Initialize empty grid
            for (int x = 0; x < COMBAT_GRID_WIDTH; x++)
                for (int y = 0; y < COMBAT_GRID_HEIGHT; y++)
                    combatGrid[x, y] = new Cell();

            // Place ship1's crew on the left side
            for (int i = 0; i < ship1.Crew.Count; i++)
            {
                int y = i % COMBAT_GRID_HEIGHT;
                ship1.Crew[i].Position = new Position(1, y);
                combatGrid[1, y].Entities.Add(ship1.Crew[i]);
            }

            // Place ship2's crew on the right side
            for (int i = 0; i < ship2.Crew.Count; i++)
            {
                int y = i % COMBAT_GRID_HEIGHT;
                ship2.Crew[i].Position = new Position(COMBAT_GRID_WIDTH - 2, y);
                combatGrid[COMBAT_GRID_WIDTH - 2, y].Entities.Add(ship2.Crew[i]);
            }

            // Select player character (first crew member of ship1)
            playerCharacter = ship1.Crew[0];
        }

        public void StartCombat()
        {
            bool combatOngoing = true;

            while (combatOngoing)
            {
                RenderCombatScene();
                HandlePlayerInput();
                CheckForCombat();
                
                // Check if combat should end
                if (ship1.Crew.Count == 0 || ship2.Crew.Count == 0)
                {
                    combatOngoing = false;
                }
            }

            AnnounceCombatResult();
        }

        private void RenderCombatScene()
        {
            Console.Clear();
            Console.WriteLine($"Ship Combat: {ship1.Name} vs {ship2.Name}");
            Console.WriteLine($"You are controlling: {playerCharacter.Name} (Health: {playerCharacter.Health})");
            Console.WriteLine("Use HJKL to move, Q to quit combat\n");

            // Draw the combat grid
            for (int y = 0; y < COMBAT_GRID_HEIGHT; y++)
            {
                for (int x = 0; x < COMBAT_GRID_WIDTH; x++)
                {
                    var cell = combatGrid[x, y];
                    if (cell.Entities.Count > 0)
                    {
                        var entity = cell.Entities[0];
                        char symbol = entity == playerCharacter ? '@' : 
                                    ship1.Crew.Contains(entity) ? 'A' : 'E';
                        Console.Write($"[{symbol}]");
                    }
                    else
                    {
                        Console.Write("[ ]");
                    }
                }
                Console.WriteLine();
            }

            // Display crew information
            Console.WriteLine($"\n{ship1.Name} Crew: {ship1.Crew.Count}");
            foreach (var crewMember in ship1.Crew)
            {
                Console.WriteLine($"{crewMember.Name}: Health={crewMember.Health}");
            }

            Console.WriteLine($"\n{ship2.Name} Crew: {ship2.Crew.Count}");
            foreach (var crewMember in ship2.Crew)
            {
                Console.WriteLine($"{crewMember.Name}: Health={crewMember.Health}");
            }
        }

        private void HandlePlayerInput()
        {
            char input = Console.ReadKey(true).KeyChar;
            Position newPosition = playerCharacter.Position;

            switch (char.ToUpper(input))
            {
                case 'K': // Up
                    newPosition = new Position(playerCharacter.Position.X, playerCharacter.Position.Y - 1);
                    break;
                case 'J': // Down
                    newPosition = new Position(playerCharacter.Position.X, playerCharacter.Position.Y + 1);
                    break;
                case 'H': // Left
                    newPosition = new Position(playerCharacter.Position.X - 1, playerCharacter.Position.Y);
                    break;
                case 'L': // Right
                    newPosition = new Position(playerCharacter.Position.X + 1, playerCharacter.Position.Y);
                    break;
                case 'Q': // Quit combat
                    ship1.Crew.Clear(); // TODO: THIS DOESN"T WORK CORRECTLY YET, RIGHT NOW IT IS JUST DEAD OF ALL CREW 
                    return;
            }

            if (IsValidPosition(newPosition))
            {
                MoveCharacter(playerCharacter, newPosition);
            }
        }

        private bool IsValidPosition(Position pos)
        {
            return pos.X >= 0 && pos.X < COMBAT_GRID_WIDTH &&
                pos.Y >= 0 && pos.Y < COMBAT_GRID_HEIGHT;
        }

        private void MoveCharacter(Character character, Position newPosition)
        {
            // Remove from old position
            combatGrid[character.Position.X, character.Position.Y].Entities.Remove(character);
            
            // Update position
            character.Position = newPosition;
            
            // Add to new position
            combatGrid[newPosition.X, newPosition.Y].Entities.Add(character);
        }

        private void CheckForCombat()
        {
            // Find adjacent enemies to player
            List<Character> adjacentEnemies = FindAdjacentEnemies(playerCharacter);
            
            foreach (var target in adjacentEnemies)
            {
                PerformAttack(playerCharacter, target);
                // Enemy counterattack
                PerformAttack(target, playerCharacter);
            }
        }

        private List<Character> FindAdjacentEnemies(Character attacker)
        {
            List<Character> enemies = new List<Character>();
            List<Position> adjacentPositions = new List<Position>
            {
                new Position(attacker.Position.X - 1, attacker.Position.Y),
                new Position(attacker.Position.X + 1, attacker.Position.Y),
                new Position(attacker.Position.X, attacker.Position.Y - 1),
                new Position(attacker.Position.X, attacker.Position.Y + 1)
            };

            foreach (var pos in adjacentPositions)
            {
                if (IsValidPosition(pos))
                {
                    var enemiesInCell = combatGrid[pos.X, pos.Y].Entities
                        .OfType<Character>()
                        .Where(c => IsEnemy(attacker, c))
                        .ToList();
                    enemies.AddRange(enemiesInCell);
                }
            }

            return enemies;
        }

        private bool IsEnemy(Character char1, Character char2)
        {
            return ship1.Crew.Contains(char1) != ship1.Crew.Contains(char2);
        }

        private void PerformAttack(Character attacker, Character target)
        {
            int damage = new Random().Next(attacker.Strength / 2, attacker.Strength);
            target.Health -= damage;
            Console.WriteLine($"{attacker.Name} attacks {target.Name} for {damage} damage!");
            
            if (target.Health <= 0)
            {
                Console.WriteLine($"{target.Name} has been defeated!");
                RemoveDefeatedCharacter(target);
            }
            
            // Pause briefly to show combat message
            Thread.Sleep(500);
        }

        private void RemoveDefeatedCharacter(Character character)
        {
            combatGrid[character.Position.X, character.Position.Y].Entities.Remove(character);
            
            if (ship1.Crew.Contains(character))
            {
                ship1.Crew.Remove(character);
                if (character == playerCharacter && ship1.Crew.Count > 0)
                {
                    playerCharacter = ship1.Crew[0];
                    Console.WriteLine($"You now control {playerCharacter.Name}!");
                    Thread.Sleep(1000);
                }
            }
            else
            {
                ship2.Crew.Remove(character);
            }
        }

        private void AnnounceCombatResult()
        {
            Console.Clear();
            if (ship1.Crew.Count == 0)
            {
                Console.WriteLine($"{ship2.Name} is victorious!");
            }
            else if (ship2.Crew.Count == 0)
            {
                Console.WriteLine($"{ship1.Name} is victorious!");
            }
            Console.WriteLine("\nPress any key to return to the main map...");
            Console.ReadKey(true);
        }
    }
}