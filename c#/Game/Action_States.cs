namespace Game
{
    // Strategy Pattern: Action Strategy Interface
    public interface IActionStrategy
    {
        void PerformAction(Character actor, Character target);
        string GetActionName();
        bool CanPerformAction(Character actor, Character target);
        int GetRange();
    }

   // Concrete Action Strategies
    public class MeleeAction : IActionStrategy
    {
        private const int STAMINA_COST = 10;
        public void PerformAction(Character actor, Character target)
        {
            int damage = new Random().Next(actor.Strength / 2, actor.Strength);
            target.TakeDamage(damage);
            actor.UseStamina(STAMINA_COST);
            GameWorld.Instance.AddToCombatLog($"{actor.Name} strikes {target.Name} for {damage} damage!");
        }
        public string GetActionName() => "Melee Attack";
        public bool CanPerformAction(Character actor, Character target) 
            => actor.Stamina >= STAMINA_COST && actor.IsAlive && target.IsAlive;
        public int GetRange() => 1;
    }

    public class RangedAction : IActionStrategy
    {
        private const int STAMINA_COST = 5;
        public void PerformAction(Character actor, Character target)
        {
            int damage = new Random().Next(actor.Strength / 3, actor.Strength / 2);
            target.TakeDamage(damage);
            actor.UseStamina(STAMINA_COST);
            actor.UseAmmunition(1);
            GameWorld.Instance.AddToCombatLog($"{actor.Name} shoots {target.Name} for {damage} damage!");
        }
        public string GetActionName() => "Ranged Attack";
        public bool CanPerformAction(Character actor, Character target) 
            => actor.Ammunition > 0 && actor.Stamina >= STAMINA_COST && actor.IsAlive && target.IsAlive;
        public int GetRange() => 3;
    }

    public class MagicAction : IActionStrategy
    {
        private const int MAGIC_COST = 15;
        public void PerformAction(Character actor, Character target)
        {
            int damage = new Random().Next(actor.MagicPoints / 2, actor.MagicPoints);
            target.TakeDamage(damage);
            actor.UseMagicPoints(MAGIC_COST);
            GameWorld.Instance.AddToCombatLog($"{actor.Name} casts a spell on {target.Name} for {damage} damage!");
        }
        public string GetActionName() => "Magic Attack";
        public bool CanPerformAction(Character actor, Character target) 
            => actor.MagicPoints >= MAGIC_COST && actor.IsAlive && target.IsAlive;
        public int GetRange() => 2;
    }

    public class HealAction : IActionStrategy
    {
        private const int MAGIC_COST = 10;
        public void PerformAction(Character actor, Character target)
        {
            int healAmount = new Random().Next(10, 20);
            target.Heal(healAmount);
            actor.UseMagicPoints(MAGIC_COST);
            GameWorld.Instance.AddToCombatLog($"{actor.Name} heals {target.Name} for {healAmount} health!");
        }
        public string GetActionName() => "Heal";
        public bool CanPerformAction(Character actor, Character target) 
            => actor.MagicPoints >= MAGIC_COST && actor.IsAlive && target.IsAlive && target.Health < target.MaxHealth;
        public int GetRange() => 2;
    }

     // State Pattern: Character State Interface
    public interface ICharacterState
    {
        void HandleState(Character character);
        string GetStateName();
    }

   // Concrete Character States
    public class IdleState : ICharacterState
    {
        public void HandleState(Character character)
        {
            character.RegenerateMagicPoints(2);
            character.RegenerateStamina(1);
        }
        public string GetStateName() => "Idle";
    }

    public class ActionState : ICharacterState
    {
        public void HandleState(Character character)
        {
            // Action state now handles stamina in the actual actions
        }
        public string GetStateName() => "Action";
    }

    public class DefendingState : ICharacterState
    {
        public void HandleState(Character character)
        {
            character.RegenerateStamina(3);
            character.RegenerateMagicPoints(1);
            GameWorld.Instance.AddToCombatLog($"{character.Name} is defending. (+3 Stamina, +1 MP)");
        }
        public string GetStateName() => "Defending";
    }
}