using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Game.Database
{
    // Base Entity for all game entities
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionX { get; set; }
        public int? PositionY { get; set; }
    }

    // Enums
    public enum EntityType
    {
        PlayerShip,
        EnemyShip,
        Location,
        Character
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Mythical
    }

    public enum QuestType
    {
        Naval,
        Combat,
        Trade,
        Escort,
        Exploration
    }

    public enum QuestState
    {
        Available,
        InProgress,
        Completed,
        Failed
    }

    public enum EquipmentSlotType
    {
        Weapon,
        Defensive,
        Utility
    }

    // Entity Models
    public class EntityModel : BaseEntity
    {
        public EntityType Type { get; set; }
        public int? ShipId { get; set; }
        public ShipModel Ship { get; set; }
        public int? LocationId { get; set; }
        public LocationModel Location { get; set; }
    }

    public class ShipModel : BaseEntity
    {
        public int Health { get; set; }
        public int AttackPower { get; set; }

        public List<CharacterModel> Crew { get; set; }
        public List<ItemModel> ShipItems { get; set; }
    }

    public class CharacterModel : BaseEntity
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }
        public int MagicPoints { get; set; }
        public int MaxMagicPoints { get; set; }
        public int Ammunition { get; set; }
        public int KilledEnemies { get; set; }

        public int? ShipId { get; set; }
        public int? LocationId { get; set; }
        public ShipModel Ship { get; set; }

        public List<ItemModel> Items { get; set; }
        public List<QuestModel> ActiveQuests { get; set; }
        public Dictionary<string, int> QuestProgress { get; set; }
    }

    public class LocationModel : BaseEntity
    {
        public int Significance { get; set; }

        public List<CharacterModel> People { get; set; }
        public List<ItemModel> LocationItems { get; set; }
    }

    public class ItemModel : BaseEntity
    {
        public ItemRarity Rarity { get; set; }
        public int? PositionX { get; set; }
        public int? PositionY { get; set; }

        public int? CharacterId { get; set; }
        public CharacterModel Character { get; set; }

        public int? ShipId { get; set; }
        public ShipModel Ship { get; set; }

        public int? LocationId { get; set; }
        public LocationModel Location { get; set; }

        public EquipmentSlotType? EquipmentSlot { get; set; }
    }

    public class QuestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public QuestState State { get; set; }

        public List<QuestObjectiveModel> Objectives { get; set; }
        public Dictionary<string, int> Rewards { get; set; }

        public int? CharacterId { get; set; }
        public CharacterModel Character { get; set; }
    }

    public class QuestObjectiveModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public int QuestId { get; set; }
        public QuestModel Quest { get; set; }
    }
}