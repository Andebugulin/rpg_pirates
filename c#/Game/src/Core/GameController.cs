namespace Game
{
    // Command Interface
    public interface ICommand
    {
        bool Execute();
        void Undo();
    }

    // Concrete Commands
    public class AttackCommand : ICommand
    {
        private readonly Character _attacker;
        private readonly Character _target;
        private readonly int _damageDealt;
        private bool _executed;

        public AttackCommand(Character attacker, Character target)
            {
            _attacker = attacker;
            _target = target;
            _damageDealt = 0;
            _executed = false;
        }

        public bool Execute()
            {
            // Check if a weapon is equipped before attacking
            var equippedWeapon = _attacker.GetEquippedItem(EquipmentSlotType.Weapon);
            if (equippedWeapon == null)
                {
                GameWorld.Instance.AddToCombatLog($"{_attacker.Name} cannot attack without a weapon!");
                return false;
                }

            if (_attacker.Stamina >= 10 && !_executed)
                {
                int damage = _attacker.GetCurrentStrategy().CalculateDamage(_attacker);
                _target.TakeDamage(damage);
                if (!_target.IsAlive)
                    {
                    _attacker.KillEnemy();
                    }
                _attacker.Stamina -= 10;
                _executed = true;
                GameWorld.Instance.AddToCombatLog($"{_attacker.Name} attacked {_target.Name} for {damage} damage with {equippedWeapon.Name}!");
                return true;
                }
            return false;
            }

        public void Undo()
            {
            if (_executed)
                {
                _target.Health += _damageDealt;
                _attacker.Stamina += 10;
                _executed = false;
                GameWorld.Instance.AddToCombatLog($"Undid {_attacker.Name}'s attack on {_target.Name}");
                }
        }
    }

    public class DefendCommand : ICommand
        {
        private readonly Character _character;
        private bool _executed;
        private ICharacterState _previousState;
        private Item _equippedDefensiveItem;

        public DefendCommand(Character character)
            {
            _character = character;
            _executed = false;
            _equippedDefensiveItem = _character.GetEquippedItem(EquipmentSlotType.Defensive);
            }

        public bool Execute()
            {
            if (_equippedDefensiveItem == null)
                {
                GameWorld.Instance.AddToCombatLog($"{_character.Name} cannot enter defensive stance without a defensive item!");
                return false;
                }

            if (!_executed)
                {
                _previousState = _character.GetCurrentState();
                _character.SetState(new DefendingState());
                _executed = true;
                GameWorld.Instance.AddToCombatLog($"{_character.Name} enters a defensive stance with {_equippedDefensiveItem.Name}!");
                return true;
                }
            return false;
            }

        public void Undo()
            {
            if (_executed)
                {
                _character.SetState(_previousState);
                _executed = false;
                GameWorld.Instance.AddToCombatLog($"{_character.Name} leaves defensive stance");
                }
            }
        }

    public class HealCommand : ICommand
    {
        private readonly Character _character;
        private readonly int _healAmount;
        private bool _executed;

        public HealCommand(Character character, int healAmount = 20)
        {
            _character = character;
            _healAmount = healAmount;
            _executed = false;
        }

        public bool Execute()
        {
            if (_character.MagicPoints >= 10 && !_executed)
            {
                int actualHeal = Math.Min(_healAmount, _character.MaxHealth - _character.Health);
                _character.Health += actualHeal;
                _character.MagicPoints -= 10;
                _executed = true;
                GameWorld.Instance.AddToCombatLog($"{_character.Name} healed for {actualHeal} HP!");
                return true;
            }
            return false;
        }

        public void Undo()
        {
            if (_executed)
            {
                _character.Health -= _healAmount;
                _character.MagicPoints += 10;
                _executed = false;
                GameWorld.Instance.AddToCombatLog($"Undid {_character.Name}'s heal");
            }
        }
    }

    public class MoveCommand : ICommand
    {
        private readonly Character _character;
        private readonly Position _newPosition;
        private Position _oldPosition;
        private readonly Cell[,] _grid;
        private bool _executed;

        public MoveCommand(Character character, Position newPosition, Cell[,] grid)
        {
            _character = character;
            _newPosition = newPosition;
            _grid = grid;
            _executed = false;
        }

        public bool Execute()
        {
            if (!_executed && IsValidMove())
            {
                _oldPosition = _character.Position;
                _grid[_oldPosition.X, _oldPosition.Y].Entities.Remove(_character);
                _character.Position = _newPosition;
                _grid[_newPosition.X, _newPosition.Y].Entities.Add(_character);
                _executed = true;
                GameWorld.Instance.AddToCombatLog($"{_character.Name} moved to position ({_newPosition.X}, {_newPosition.Y})");
                return true;
            }
            return false;
        }

        public void Undo()
        {
            if (_executed)
            {
                _grid[_newPosition.X, _newPosition.Y].Entities.Remove(_character);
                _character.Position = _oldPosition;
                _grid[_oldPosition.X, _oldPosition.Y].Entities.Add(_character);
                _executed = false;
                GameWorld.Instance.AddToCombatLog($"Undid {_character.Name}'s movement");
            }
        }

        private bool IsValidMove()
        {
            return _newPosition.X >= 0 && _newPosition.X < _grid.GetLength(0) &&
                   _newPosition.Y >= 0 && _newPosition.Y < _grid.GetLength(1) &&
                   !_grid[_newPosition.X, _newPosition.Y].Entities.Any(e => e is Character);
        }
    }

    // Macro Command for combining multiple commands
    public class MacroCommand : ICommand
    {
        private readonly List<ICommand> _commands;

        public MacroCommand(List<ICommand> commands)
        {
            _commands = commands;
        }

        public bool Execute()
        {
            bool success = true;
            foreach (var command in _commands)
            {
                if (!command.Execute())
                {
                    success = false;
                    break;
                }
            }
            return success;
        }

        public void Undo()
        {
            // Undo commands in reverse order
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }

    // Game Controller (Invoker)
    public class GameController
    {
        private readonly Stack<ICommand> _commandHistory;
        private readonly Stack<ICommand> _undoneCommands;

        public GameController()
        {
            _commandHistory = new Stack<ICommand>();
            _undoneCommands = new Stack<ICommand>();
        }

        public bool ExecuteCommand(ICommand command)
        {
            if (command.Execute())
            {
                _commandHistory.Push(command);
                _undoneCommands.Clear(); // Clear redo stack when new command is executed
                return true;
            }
            return false;
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count > 0)
            {
                var command = _commandHistory.Pop();
                command.Undo();
                _undoneCommands.Push(command);
            }
        }

        public void RedoLastCommand()
        {
            if (_undoneCommands.Count > 0)
            {
                var command = _undoneCommands.Pop();
                if (command.Execute())
                {
                    _commandHistory.Push(command);
                }
            }
        }
    }
}