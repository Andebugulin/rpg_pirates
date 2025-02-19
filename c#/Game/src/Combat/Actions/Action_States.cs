namespace Game
{
    // Strategy Pattern: Action Strategy Interface
    public interface IActionStrategy
    {
        void PerformAction(Character actor, Character target);
        string GetActionName();
        bool CanPerformAction(Character actor, Character target);
        int GetRange();
        int CalculateDamage(Character actor);
    }

   // Concrete Action Strategies
    public class MeleeAction : IActionStrategy
    {
    private const int STAMINA_COST = 10;
    public void PerformAction(Character actor, Character target)
        {
        int damage = CalculateDamage(actor);
        target.TakeDamage(damage);
        actor.UseStamina(STAMINA_COST);
        GameWorld.Instance.AddToCombatLog($"{actor.Name} strikes {target.Name} for {damage} damage!");
        }

    public int CalculateDamage(Character actor)
        {
        var weapon = actor.GetEquippedItem(EquipmentSlotType.Weapon) as Weapon;
        if (weapon == null)
            {
            // Unarmed damage (minimal)
            return Math.Max(1, actor.Strength / 4);
            }


        // Base damage on weapon's damage + some character strength variation
        return weapon.Damage + (actor.Strength / 4);
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
        int damage = CalculateDamage(actor);
        target.TakeDamage(damage);
        actor.UseStamina(STAMINA_COST);
        actor.UseAmmunition(1);
        GameWorld.Instance.AddToCombatLog($"{actor.Name} shoots {target.Name} for {damage} damage!");
        }

    public int CalculateDamage(Character actor)
        {
        var weapon = actor.GetEquippedItem(EquipmentSlotType.Weapon) as Weapon;
        if (weapon == null)
            {
            // Unarmed ranged damage (minimal)
            return Math.Max(1, actor.Strength / 6);
            }


        // Base damage on weapon's damage with slight reduction for ranged
        return weapon.Damage * 3 / 4 + (actor.Strength / 6);
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
        int damage = CalculateDamage(actor);
        target.TakeDamage(damage);
        actor.UseMagicPoints(MAGIC_COST);
        GameWorld.Instance.AddToCombatLog($"{actor.Name} casts a spell on {target.Name} for {damage} damage!");
        }

    public int CalculateDamage(Character actor)
        {
        var relic = actor.GetEquippedItem(EquipmentSlotType.Utility) as Relic;
        if (relic == null)
            {
            // Base magic damage without a relic
            return Math.Max(1, actor.MagicPoints / 3);
            }


        // Base damage on relic's power + magic points
        return relic.Power + (actor.MagicPoints / 3);
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
        public int CalculateDamage(Character actor) => new Random().Next(10, 20);
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