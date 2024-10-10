namespace Game 
{
    // Base class for all characters
   public abstract class Character : Entity
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }
        public int MagicPoints { get; set; }
        public int MaxMagicPoints { get; set; }
        public int Ammunition { get; set; }
        
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
            Ammunition = 10;
            
            availableStrategies = new List<IActionStrategy>
            {
                new MeleeAction(),
                new RangedAction(),
                new HealAction()
            };
            
            currentStrategy = availableStrategies[0]; // Default to melee
            currentState = new IdleState(); // Start in idle state
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
        
        public override char[,] GetDisplayTile()
        {
            return new char[,]
            {
                {Name[0]},
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
