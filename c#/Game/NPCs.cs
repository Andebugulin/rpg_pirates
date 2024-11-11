namespace Game 
{
   // Updated Character class
    public abstract class Character : Entity, IQuestObserver
    {
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int Strength { get; protected set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; protected set; }
        public int MagicPoints { get; set; }
        public int MaxMagicPoints { get; protected set; }
        public int Ammunition { get; protected set; }
        public bool IsAlive => Health > 0;
        public List<Item> Items { get; private set; }

        private IActionStrategy currentStrategy;
        private ICharacterState currentState;
        private List<IActionStrategy> availableStrategies;

        public List<Quest> ActiveQuests { get; private set; }
        public Dictionary<string, int> QuestProgress { get; private set; }

        public Character(string name, Position position, int health, int strength) 
            : base(name, position, EntityType.Character)
        {
            Health = MaxHealth = health;
            Strength = strength;
            Stamina = MaxStamina = 100;
            MagicPoints = MaxMagicPoints = 50;
            Ammunition = new Random().Next(1, 2);
            Items = new List<Item>();
            
            availableStrategies = new List<IActionStrategy>
            {
                new HealAction()
            };
            currentStrategy = availableStrategies[0];
            currentState = new IdleState();

            ActiveQuests = new List<Quest>();
            QuestProgress = new Dictionary<string, int>();
        }

        public void AcceptQuest(Quest quest)
        {
            if (!ActiveQuests.Contains(quest))
            {
                ActiveQuests.Add(quest);
                foreach (var objective in quest.Objectives)
                {
                    QuestProgress[objective.Description] = 0;
                }
                GameWorld.Instance.AddToCombatLog($"{Name} accepted quest: {quest.Name}");
            }
        }
        public void AbandonQuest(Quest quest)
        {
            if (ActiveQuests.Contains(quest))
            {
                ActiveQuests.Remove(quest);
                foreach (var objective in quest.Objectives)
                {
                    QuestProgress.Remove(objective.Description);
                }
                GameWorld.Instance.AddToCombatLog($"{Name} abandoned quest: {quest.Name}");
            }
        }
         public void OnQuestStarted(Quest quest)
    {
        GameWorld.Instance.AddToCombatLog($"{Name} received new quest: {quest.Name}");
        GameWorld.Instance.AddToCombatLog($"Description: {quest.Description}");
        
        // Display rewards
        foreach (var reward in quest.Rewards)
        {
            GameWorld.Instance.AddToCombatLog($"Reward: {reward.Value} {reward.Key}");
        }
    }

    public void OnQuestCompleted(Quest quest)
    {
        GameWorld.Instance.AddToCombatLog($"{Name} completed quest: {quest.Name}!");
        
        // Process rewards
        foreach (var reward in quest.Rewards)
        {
            switch (reward.Key)
            {
                case "Gold":
                    GameWorld.Instance.AddToCombatLog($"{Name} received {reward.Value} gold");
                    break;
                case "Experience":
                    GameWorld.Instance.AddToCombatLog($"{Name} gained {reward.Value} experience");
                    break;
                case "Reputation":
                    GameWorld.Instance.AddToCombatLog($"{Name} gained {reward.Value} reputation");
                    break;
            }
        }
        
        ActiveQuests.Remove(quest);
    }

    public void OnQuestObjectiveUpdated(Quest quest, QuestObjective objective)
    {
        if (ActiveQuests.Contains(quest))
        {
            GameWorld.Instance.AddToCombatLog($"{Name} updated objective: {objective.Description}");
            if (objective.IsCompleted)
            {
                GameWorld.Instance.AddToCombatLog($"{Name} completed objective: {objective.Description}");
            }
        }
    }

    public void OnQuestUpdated(Quest quest, string message)
    {
        if (ActiveQuests.Contains(quest))
        {
            GameWorld.Instance.AddToCombatLog($"{Name} - {message}");
        }
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
                UpdateQuestProgress(target);
                return true;    
            }
            return false;
        }

        private void UpdateQuestProgress(Character target)
        {
            foreach (var quest in ActiveQuests)
            {
                foreach (var objective in quest.Objectives.Where(o => !o.IsCompleted))
                {
                    switch (quest.Type)
                    {
                        case QuestType.Combat when target is Pirate && currentStrategy.GetActionName().Contains("Attack"):
                            IncrementProgress(objective, "Defeat pirates");
                            break;
                        case QuestType.Naval when target.GetType().Name.Contains("Soldier"):
                            IncrementProgress(objective, "Defeat enemy soldiers");
                            break;
                    }
                }
            }
        }
        private void IncrementProgress(QuestObjective objective, string progressKey)
        {
            if (QuestProgress.ContainsKey(progressKey))
            {
                QuestProgress[progressKey]++;
                GameWorld.Instance.AddToCombatLog($"{Name} made progress on objective: {progressKey}");
            }
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
