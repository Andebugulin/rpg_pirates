namespace Game 
{
   // Updated Character class
    public abstract class Character : Entity
    {
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int Strength { get; protected set; }
        public int Stamina { get; protected set; }
        public int MaxStamina { get; protected set; }
        public int MagicPoints { get; protected set; }
        public int MaxMagicPoints { get; protected set; }
        public int Ammunition { get; protected set; }
        public bool IsAlive => Health > 0;
        public List<Item> Items { get; private set; }

        private IActionStrategy currentStrategy;
        private ICharacterState currentState;
        private List<IActionStrategy> availableStrategies;

        public Character(string name, Position position, int health, int strength) 
            : base(name, position, EntityType.Character)
        {
            Health = MaxHealth = health;
            Strength = strength;
            Stamina = MaxStamina = 100;
            MagicPoints = MaxMagicPoints = 50;
            Ammunition =  new Random().Next(1, 2);
            Items = new List<Item>();
            

            availableStrategies = new List<IActionStrategy>
            {
                new HealAction()
            };
            currentStrategy = availableStrategies[0]; // Default to melee
            currentState = new IdleState(); // Start in idle state
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }
        public void AddStrategy(IActionStrategy strategy)
        {
            availableStrategies.Add(strategy);
        }

        public void SetStrategy(IActionStrategy strategy)
        {
            currentStrategy = strategy;
            SetState(new ActionState());
        }

        public void SetState(ICharacterState state)
        {
            currentState = state;
        }

        public bool PerformAction(Character target)
        {
            if (currentStrategy.CanPerformAction(this, target))
            {
                currentStrategy.PerformAction(this, target);
                return true;
            }
            return false;
        }

        public void UpdateState()
        {
            currentState.HandleState(this);
        }

        public List<IActionStrategy> GetAvailableStrategies() => availableStrategies;
        public IActionStrategy GetCurrentStrategy() => currentStrategy;
        public ICharacterState GetCurrentState() => currentState;

        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);
            if (Health <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public void UseStamina(int amount)
        {
            Stamina = Math.Max(0, Stamina - amount);
        }

        public void UseMagicPoints(int amount)
        {
            MagicPoints = Math.Max(0, MagicPoints - amount);
        }

        public void UseAmmunition(int amount)
        {
            Ammunition = Math.Max(0, Ammunition - amount);
        }

        public void RegenerateStamina(int amount)
        {
            Stamina = Math.Min(MaxStamina, Stamina + amount);
        }

        public void RegenerateMagicPoints(int amount)
        {
            MagicPoints = Math.Min(MaxMagicPoints, MagicPoints + amount);
        }

        private void Die()
        {
            GameWorld.Instance.AddToCombatLog($"{Name} has been defeated!");
            // Remove from the game world
            GameWorld.Instance.RemoveEntity(this);
        }

        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {IsAlive ? Name[0] : 'X'},
                {currentState.GetStateName()[0]}
            };
        }
    }

    public class Civilian : Character
    {
        public Civilian(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

    }

    public class SpanishSoldier : Character
    {
        public SpanishSoldier(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

    }

    public class EnglishSoldier : Character
    {
        public EnglishSoldier(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

    }

    public class Pirate : Character
    {
        public Pirate(string name, Position position, int health, int strength) 
            : base(name, position, health, strength) 
        { }

    }
}
