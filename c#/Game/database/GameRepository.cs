using Microsoft.EntityFrameworkCore;

namespace Game.Database
{
    
    // Repository Pattern for Data Access
    public class GameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository(GameDbContext context)
        {
            _context = context;
        }

        // Generic CRUD methods
        public async Task<T> AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Specific methods for game logic
        public async Task<CharacterModel> AddItemToCharacterInventory(int characterId, ItemModel item)
        {
            var character = await _context.Characters
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == characterId);

            if (character == null)
                throw new ArgumentException("Character not found");

            character.Items.Add(item);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<QuestModel> AddQuestToCharacter(int characterId, QuestModel quest)
        {
            var character = await _context.Characters
                .Include(c => c.ActiveQuests)
                .FirstOrDefaultAsync(c => c.Id == characterId);

            if (character == null)
                throw new ArgumentException("Character not found");

            character.ActiveQuests.Add(quest);
            await _context.SaveChangesAsync();
            return quest;
        }
    }
}