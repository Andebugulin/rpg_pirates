using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Game.Database
{
    public class GameDbContext : DbContext
    {
        // Constructor for dependency injection
        public GameDbContext(DbContextOptions<GameDbContext> options) 
            : base(options)
        {
        }

        // DbSet properties remain the same
        public DbSet<EntityModel> Entities { get; set; }
        public DbSet<ShipModel> Ships { get; set; }
        public DbSet<CharacterModel> Characters { get; set; }
        public DbSet<LocationModel> Locations { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<QuestModel> Quests { get; set; }
        public DbSet<QuestObjectiveModel> QuestObjectives { get; set; }

        // Remove OnConfiguring method

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Your existing configuration remains the same
            modelBuilder.Entity<CharacterModel>()
            .Property(c => c.QuestProgress)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );

            modelBuilder.Entity<QuestModel>()
                .Property(q => q.Rewards)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, (System.Text.Json.JsonSerializerOptions?)null)
                );


            // One-to-Many relationships
            modelBuilder.Entity<ShipModel>()
                .HasMany(s => s.Crew)
                .WithOne(c => c.Ship)
                .HasForeignKey(c => c.ShipId);

            modelBuilder.Entity<ShipModel>()
                .HasMany(s => s.ShipItems)
                .WithOne(i => i.Ship)
                .HasForeignKey(i => i.ShipId);

            modelBuilder.Entity<LocationModel>()
                .HasMany(l => l.People)
                .WithOne()
                .HasForeignKey(c => c.LocationId);

            modelBuilder.Entity<LocationModel>()
                .HasMany(l => l.LocationItems)
                .WithOne(i => i.Location)
                .HasForeignKey(i => i.LocationId);

            // Quest and Objective relationship
            modelBuilder.Entity<QuestModel>()
                .HasMany(q => q.Objectives)
                .WithOne(o => o.Quest)
                .HasForeignKey(o => o.QuestId);
        }
    }
}