namespace Game
{
    public class CrewCombatScene
    {
        private const int CombatGridWidth = 15;
        private const int CombatGridHeight = 10;
        private const int SpaceBetweenCrews = 5;

        private readonly Ship _ship1;
        private readonly Ship _ship2;
        private readonly Cell[,] _combatGrid;
        private Character _playerCharacter;

        public CrewCombatScene(Ship ship1, Ship ship2)
        {
            _ship1 = ship1;
            _ship2 = ship2;
            _combatGrid = new Cell[CombatGridWidth, CombatGridHeight];
            InitializeCombatGrid();
        }

        public void StartCombat()
        {
            bool combatOngoing = true;
            while (combatOngoing)
            {
                RenderCombatScene();
                HandlePlayerInput();
                HandleAIActions();
                UpdateCharacterStates();
                combatOngoing = !IsCombatOver();
            }
            AnnounceCombatResult();
        }

        private void InitializeCombatGrid()
        {
            for (int x = 0; x < CombatGridWidth; x++)
                for (int y = 0; y < CombatGridHeight; y++)
                    _combatGrid[x, y] = new Cell();

            PlaceCrewOnGrid(_ship1.Crew, 1);
            PlaceCrewOnGrid(_ship2.Crew, CombatGridWidth - SpaceBetweenCrews);

            _playerCharacter = _ship1.Crew[0];
        }

        private void PlaceCrewOnGrid(List<Character> crew, int xPosition)
        {
            for (int i = 0; i < crew.Count; i++)
            {
                int y = i * 2 % CombatGridHeight;
                crew[i].Position = new Position(xPosition, y);
                _combatGrid[xPosition, y].Entities.Add(crew[i]);
            }
        }

        private void RenderCombatScene()
        {
            Console.Clear();
            DisplayCombatHeader();
            DrawCombatGrid();
            DisplayCombatLog();
            DisplayCrewStatus(_ship1, "Ship 1");
            DisplayCrewStatus(_ship2, "Ship 2");
            DisplayPlayerCharacterDetails();
        }

        private void DisplayCombatHeader()
        {
            Console.WriteLine($"Ship Combat: {_ship1.Name} vs {_ship2.Name}");
            Console.WriteLine($"You are controlling: {_playerCharacter.Name} (Health: {_playerCharacter.Health}/{_playerCharacter.MaxHealth})");
            Console.WriteLine("Use HJKL to move, SPACE to perform action, 1-4 to change strategy, D to defend, Q to quit combat\n");
        }

        private void DrawCombatGrid()
        {
            for (int y = 0; y < CombatGridHeight; y++)
            {
                for (int x = 0; x < CombatGridWidth; x++)
                {
                    var cell = _combatGrid[x, y];
                    if (cell.Entities.Count > 0)
                    {
                        var entity = cell.Entities[0] as Character;
                        char symbol = entity == _playerCharacter ? '@' : 
                                    _ship1.Crew.Contains(entity) ? 'A' : 'E';
                        Console.Write($"[{symbol}]");
                    }
                    else
                    {
                        Console.Write("[ ]");
                    }
                }
                Console.WriteLine();
            }
        }

        private void DisplayCombatLog()
        {
            Console.WriteLine("\nCombat Log:");
            foreach (var logEntry in GameWorld.Instance.combatLog.TakeLast(5))
            {
                Console.WriteLine(logEntry);
            }
        }

        private void DisplayCrewStatus(Ship ship, string shipName)
        {
            Console.WriteLine($"\n{shipName} Crew:");
            foreach (var crew in ship.Crew)
            {
                Console.WriteLine($"{crew.Name}: HP {crew.Health}/{crew.MaxHealth} | SP {crew.Stamina}/{crew.MaxStamina} | MP {crew.MagicPoints}/{crew.MaxMagicPoints}");
            }
        }

        private void DisplayPlayerCharacterDetails()
        {
            Console.WriteLine($"\nCurrent Action: {_playerCharacter.GetCurrentStrategy().GetActionName()}");
            Console.WriteLine($"Current State: {_playerCharacter.GetCurrentState().GetStateName()}");
            Console.WriteLine($"Stamina: {_playerCharacter.Stamina}/{_playerCharacter.MaxStamina}");
            Console.WriteLine($"Magic Points: {_playerCharacter.MagicPoints}/{_playerCharacter.MaxMagicPoints}");
            Console.WriteLine($"Ammunition: {_playerCharacter.Ammunition}");
        }

        private void HandlePlayerInput()
        {
            Console.WriteLine("\nActions:");
            Console.WriteLine("1-4: Select action strategy");
            Console.WriteLine("HJKL: Move");
            Console.WriteLine("SPACE: Perform current action");
            Console.WriteLine("D: Enter defending state");
            Console.WriteLine("Q: Quit combat");

            char input = Console.ReadKey(true).KeyChar;
            if (char.IsDigit(input))
            {
                HandleStrategySelection(input);
                return;
            }

            switch (char.ToUpper(input))
            {
                case 'K':
                case 'J':
                case 'H':
                case 'L':
                    HandleMovement(input);
                    break;
                case 'D':
                    _playerCharacter.SetState(new DefendingState());
                    Console.WriteLine($"{_playerCharacter.Name} is now defending!");
                    break;
                case ' ':
                    HandleAction();
                    break;
                case 'Q':
                    _ship1.Crew.Clear(); // Force end combat
                    break;
            }
        }

        private void HandleStrategySelection(char input)
        {
            int index = int.Parse(input.ToString()) - 1;
            var strategies = _playerCharacter.GetAvailableStrategies();
            if (index >= 0 && index < strategies.Count)
            {
                _playerCharacter.SetStrategy(strategies[index]);
                Console.WriteLine($"Switched to {strategies[index].GetActionName()}");
            }
        }

        private void HandleMovement(char input)
        {
            Position newPosition = GetNewPosition(input);
            if (IsValidPosition(newPosition))
            {
                MoveCharacter(_playerCharacter, newPosition);
                _playerCharacter.SetState(new ActionState());
            }
        }

        private Position GetNewPosition(char input)
        {
            switch (char.ToUpper(input))
            {
                case 'K': return new Position(_playerCharacter.Position.X, _playerCharacter.Position.Y - 1);
                case 'J': return new Position(_playerCharacter.Position.X, _playerCharacter.Position.Y + 1);
                case 'H': return new Position(_playerCharacter.Position.X - 1, _playerCharacter.Position.Y);
                case 'L': return new Position(_playerCharacter.Position.X + 1, _playerCharacter.Position.Y);
                default: return _playerCharacter.Position;
            }
        }

        private void HandleAction()
        {
            var currentStrategy = _playerCharacter.GetCurrentStrategy();
            int actionRange = currentStrategy.GetRange();
            List<Character> possibleTargets = FindTargetsInRange(_playerCharacter, actionRange);

            if (possibleTargets.Count > 0)
            {
                Character target = possibleTargets[0];
                if (_playerCharacter.PerformAction(target))
                {
                    if (target.Health <= 0)
                    {
                        RemoveDefeatedCharacter(target);
                    }
                }
                else
                {
                    Console.WriteLine("Cannot perform this action!");
                }
            }
            else
            {
                Console.WriteLine("No targets in range!");
            }
        }

        private void HandleAIActions()
        {
            // Handle AI actions for Ship 1 crew (excluding the player character)
            foreach (var character in _ship1.Crew.Where(c => c != _playerCharacter && c.IsAlive))
            {
                HandleAIAction(character, _ship2.Crew);
            }

            // Handle AI actions for Ship 2 crew
            foreach (var character in _ship2.Crew.Where(c => c.IsAlive))
            {
                HandleAIAction(character, _ship1.Crew);
            }
        }

        private void HandleAIAction(Character aiCharacter, List<Character> enemyCrew)
        {
            var closestEnemy = FindClosestEnemy(aiCharacter, enemyCrew);
            if (closestEnemy == null) return;

            int distance = CalculateDistance(aiCharacter.Position, closestEnemy.Position);
            var strategies = aiCharacter.GetAvailableStrategies();

            if (ShouldHeal(aiCharacter, strategies))
            {
                PerformHealAction(aiCharacter);
                return;
            }

            IActionStrategy selectedStrategy = ChooseStrategy(aiCharacter, distance, strategies);
            if (selectedStrategy != null)
            {
                aiCharacter.SetStrategy(selectedStrategy);
                if (aiCharacter.PerformAction(closestEnemy))
                    return;
            }

            MoveTowardsTarget(aiCharacter, closestEnemy.Position);
        }

        private bool ShouldHeal(Character aiCharacter, List<IActionStrategy> strategies)
        {
            return aiCharacter.Health < aiCharacter.MaxHealth / 2 && 
                   strategies.Any(s => s is HealAction) && 
                   aiCharacter.MagicPoints >= 10;
        }

        private void PerformHealAction(Character aiCharacter)
        {
            aiCharacter.SetStrategy(aiCharacter.GetAvailableStrategies().First(s => s is HealAction));
            aiCharacter.PerformAction(aiCharacter);
        }

        private IActionStrategy ChooseStrategy(Character aiCharacter, int distance, List<IActionStrategy> strategies)
        {
            if (distance <= 1 && strategies.Any(s => s is MeleeAction))
                return strategies.First(s => s is MeleeAction);
            if (distance <= 3 && aiCharacter.Ammunition > 0 && strategies.Any(s => s is RangedAction))
                return strategies.First(s => s is RangedAction);
            if (distance <= 2 && aiCharacter.MagicPoints >= 15 && strategies.Any(s => s is MagicAction))
                return strategies.First(s => s is MagicAction);
            return null;
        }

        private void UpdateCharacterStates()
        {
            foreach (var crew in _ship1.Crew.Concat(_ship2.Crew))
            {
                crew.UpdateState();
            }
        }

        private bool IsCombatOver()
        {
            return _ship1.Crew.All(c => !c.IsAlive) || _ship2.Crew.All(c => !c.IsAlive);
        }

        private void AnnounceCombatResult()
        {
            Console.Clear();
            if (_ship1.Crew.Count == 0 && _ship2.Crew.Count == 0)
            {
                Console.WriteLine("The battle ends in a draw! Both crews have been defeated.");
            }
            else if (_ship1.Crew.Count == 0)
            {
                Console.WriteLine($"{_ship2.Name} is victorious!");
            }
            else if (_ship2.Crew.Count == 0)
            {
                Console.WriteLine($"{_ship1.Name} is victorious!");
            }
            Console.WriteLine("\nPress any key to return to the main map...");
            Console.ReadKey(true);
        }

        private bool IsValidPosition(Position pos)
        {
            return pos.X >= 0 && pos.X < CombatGridWidth &&
                   pos.Y >= 0 && pos.Y < CombatGridHeight;
        }

        private void MoveCharacter(Character character, Position newPosition)
        {
            _combatGrid[character.Position.X, character.Position.Y].Entities.Remove(character);
            character.Position = newPosition;
            _combatGrid[newPosition.X, newPosition.Y].Entities.Add(character);
        }

        private Character FindClosestEnemy(Character character, List<Character> enemies)
        {
            return enemies
                .Where(e => e != character && e.IsAlive)
                .OrderBy(e => CalculateDistance(character.Position, e.Position))
                .FirstOrDefault();
        }

        private void MoveTowardsTarget(Character character, Position target)
        {
            Position newPos = new Position(character.Position.X, character.Position.Y);

            if (target.X > character.Position.X) newPos.X++;
            else if (target.X < character.Position.X) newPos.X--;

            if (target.Y > character.Position.Y) newPos.Y++;
            else if (target.Y < character.Position.Y) newPos.Y--;

            if (IsValidPosition(newPos) && !IsPositionOccupied(newPos))
            {
                MoveCharacter(character, newPos);
            }
        }

        private bool IsPositionOccupied(Position pos)
        {
            return _combatGrid[pos.X, pos.Y].Entities.Any(e => e is Character);
        }

        private int CalculateDistance(Position pos1, Position pos2)
        {
            return Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);
        }

        private List<Character> FindTargetsInRange(Character attacker, int range)
        {
            List<Character> enemyCrew = attacker == _ship1.Crew[0] ? _ship2.Crew : _ship1.Crew;
            return enemyCrew.Where(target => CalculateDistance(attacker.Position, target.Position) <= range).ToList();
        }

        private void RemoveDefeatedCharacter(Character character)
        {
            _combatGrid[character.Position.X, character.Position.Y].Entities.Remove(character);

            if (_ship1.Crew.Contains(character))
            {
                _ship1.Crew.Remove(character);
                if (character == _playerCharacter && _ship1.Crew.Count > 0)
                {
                    _playerCharacter = _ship1.Crew[0];
                    Console.WriteLine($"You now control {_playerCharacter.Name}!");
                    Thread.Sleep(1000);
                }
            }
            else
            {
                _ship2.Crew.Remove(character);
            }
        }
    }
}