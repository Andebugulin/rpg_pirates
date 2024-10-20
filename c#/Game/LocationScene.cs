namespace Game
{
    public class LocationScene
    {
        private Location _location;
        private Ship _playerShip;
        private Character _playerCharacter;
        private const int SceneWidth = 20;
        private const int SceneHeight = 10;
        private Cell[,] _sceneGrid;

        public LocationScene(Location location, Ship playerShip)
        {
            _location = location;
            _playerShip = playerShip;
            _playerCharacter = _playerShip.Crew[0]; // Assume the first crew member is the player
            _sceneGrid = new Cell[SceneWidth, SceneHeight];
            InitializeScene();
        }

        private void InitializeScene()
        {
            for (int x = 0; x < SceneWidth; x++)
            {
                for (int y = 0; y < SceneHeight; y++)
                {
                    _sceneGrid[x, y] = new Cell();
                }
            }

            // Place player character
            _playerCharacter.Position = new Position(SceneWidth / 2, SceneHeight / 2);
            _sceneGrid[_playerCharacter.Position.X, _playerCharacter.Position.Y].Entities.Add(_playerCharacter);

            // Place NPCs randomly
            foreach (var person in _location.People)
            {
                Position pos;
                do
                {
                    pos = new Position(Utils.GetRandomPosition(SceneWidth, SceneHeight).X, Utils.GetRandomPosition(SceneWidth, SceneHeight).Y);
                } while (_sceneGrid[pos.X, pos.Y].Entities.Count > 0);

                person.Position = pos;
                _sceneGrid[pos.X, pos.Y].Entities.Add(person);
            }

            // Place items randomly
            foreach (var item in _location.LocationItems)
            {
                Position pos;
                do
                {
                    pos = new Position(Utils.GetRandomPosition(SceneWidth, SceneHeight).X, Utils.GetRandomPosition(SceneWidth, SceneHeight).Y);
                } while (_sceneGrid[pos.X, pos.Y].Entities.Count > 0 || _sceneGrid[pos.X, pos.Y].Items.Count > 0);

                item.Position = pos;
                _sceneGrid[pos.X, pos.Y].Items.Add(item);
            }
        }

        public void Start()
        {
            bool isRunning = true;
            while (isRunning)
            {
                RenderScene();
                isRunning = HandleInput();
            }
        }

        private void RenderScene()
        {
            Console.Clear();
            Console.WriteLine($"Location: {_location.Name}");
            for (int y = 0; y < SceneHeight; y++)
            {
                for (int x = 0; x < SceneWidth; x++)
                {
                    if (_sceneGrid[x, y].Entities.Count > 0)
                    {
                        Console.Write(_sceneGrid[x, y].Entities[0] is Character c ? c.Name[0] : 'E');
                    }
                    else if (_sceneGrid[x, y].Items.Count > 0)
                    {
                        Console.Write('I');
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("\nControls: HJKL to move, SPACE to interact, Q to quit");
        }

        private bool HandleInput()
        {
            var key = Console.ReadKey(true).Key;
            Position newPos = new Position(_playerCharacter.Position.X, _playerCharacter.Position.Y);

            switch (key)
            {
                case ConsoleKey.K: newPos.Y--; break;
                case ConsoleKey.J: newPos.Y++; break;
                case ConsoleKey.H:newPos.X--; break;
                case ConsoleKey.L: newPos.X++; break;
                case ConsoleKey.Spacebar: Interact(); return true;
                case ConsoleKey.Q: return false;
            }

            if (IsValidMove(newPos))
            {
                MovePlayer(newPos);
            }

            return true;
        }

        private bool IsValidMove(Position pos)
        {
            return pos.X >= 0 && pos.X < SceneWidth && pos.Y >= 0 && pos.Y < SceneHeight &&
                _sceneGrid[pos.X, pos.Y].Entities.Count == 0;
        }

        private void MovePlayer(Position newPos)
        {
            _sceneGrid[_playerCharacter.Position.X, _playerCharacter.Position.Y].Entities.Remove(_playerCharacter);
            _playerCharacter.Position = newPos;
            _sceneGrid[newPos.X, newPos.Y].Entities.Add(_playerCharacter);
        }

        private void Interact()
        {
            var adjacentPositions = new List<Position>
            {
                new Position(_playerCharacter.Position.X, _playerCharacter.Position.Y - 1),
                new Position(_playerCharacter.Position.X, _playerCharacter.Position.Y + 1),
                new Position(_playerCharacter.Position.X - 1, _playerCharacter.Position.Y),
                new Position(_playerCharacter.Position.X + 1, _playerCharacter.Position.Y),
                _playerCharacter.Position
                
            };

            foreach (var pos in adjacentPositions)
            {
                if (IsValidPosition(pos))
                {
                    if (_sceneGrid[pos.X, pos.Y].Entities.Count > 0)
                    {
                        var entity = _sceneGrid[pos.X, pos.Y].Entities[0];
                        if (entity is Character c && entity.Position != _playerCharacter.Position)
                        {
                            Console.WriteLine($"Talking to {c.Name}");
                            Console.ReadKey(true);
                        }
                    }
                    if (_sceneGrid[pos.X, pos.Y].Items.Count > 0)
                    {
                        var item = _sceneGrid[pos.X, pos.Y].Items[0];
                        Console.WriteLine($"Picked up {item.Name}");
                        _playerCharacter.AddItem(item);
                        _sceneGrid[pos.X, pos.Y].Items.Remove(item);
                        _location.LocationItems.Remove(item);
                        Console.ReadKey(true);
                    }
                }
            }
        }

        private bool IsValidPosition(Position pos)
        {
            return pos.X >= 0 && pos.X < SceneWidth && pos.Y >= 0 && pos.Y < SceneHeight;
        }
    }
}