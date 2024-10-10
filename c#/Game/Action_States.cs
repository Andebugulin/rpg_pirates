namespace Game
{
    public interface IActionStrategy
    {
        void PerformAction(Character actor, Character target);
        string GetActionName();
        bool CanPerformAction(Character actor, Character target);
        int GetRange(); // How many cells away the action can be performed
    }

    public class MeleeAction : IActionStrategy
    {
        public void PerformAction(Character actor, Character target)
        {
            int damage = new Random().Next(actor.Strength / 2, actor.Strength);
            target.Health -= damage;
            Console.WriteLine($"{actor.Name} performs a melee attack on {target.Name} for {damage} damage!");
        }

        public string GetActionName() => "Melee Attack";
        
        public bool CanPerformAction(Character actor, Character target) => true;
        
        public int GetRange() => 1; // Adjacent cells only
    }
    public class RangedAction : IActionStrategy
    {
        public void PerformAction(Character actor, Character target)
        {
            int damage = new Random().Next(actor.Strength / 3, actor.Strength / 2);
            target.Health -= damage;
            Console.WriteLine($"{actor.Name} shoots at {target.Name} for {damage} damage!");
        }

        public string GetActionName() => "Ranged Attack";
        
        public bool CanPerformAction(Character actor, Character target) 
            => actor.Ammunition > 0;
        
        public int GetRange() => 3; // Can attack up to 3 cells away
    }

    public class HealAction : IActionStrategy
    {
        public void PerformAction(Character actor, Character target)
        {
            int healAmount = new Random().Next(10, 20);
            target.Health += healAmount;
            Console.WriteLine($"{actor.Name} heals {target.Name} for {healAmount} health!");
        }

        public string GetActionName() => "Heal";
        
        public bool CanPerformAction(Character actor, Character target) 
            => actor.MagicPoints >= 10;
        
        public int GetRange() => 2; // Can heal up to 2 cells away
    }

    // State Pattern implementations
    public interface ICharacterState
    {
        void HandleState(Character character);
        string GetStateName();
    }

    public class IdleState : ICharacterState
    {
        public void HandleState(Character character)
        {
            // In idle state, character regenerates some magic points
            if (character.MagicPoints < character.MaxMagicPoints)
            {
                character.MagicPoints += 1;
            }
        }

        public string GetStateName() => "Idle";
    }

    public class ActionState : ICharacterState
    {
        public void HandleState(Character character)
        {
            // In action state, character uses more stamina
            character.Stamina -= 1;
        }

        public string GetStateName() => "Action";
    }

    public class DefendingState : ICharacterState
    {
        public void HandleState(Character character)
        {
            // In defending state, character regenerates stamina
            if (character.Stamina < character.MaxStamina)
            {
                character.Stamina += 2;
            }
        }

        public string GetStateName() => "Defending";
    }
}