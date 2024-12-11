namespace Game
{
    public static class CollectionExtensions
    {
        public static bool Add<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dict,
            TKey key,
            Func<TKey, TValue> valueFactory)
        {
            if (!dict.TryGetValue(key, out TValue value))
            {
                value = valueFactory(key);
                dict[key] = value;
            }
            return value;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
    public enum QuestType
    {
        Naval,      // Ship-based missions
        Combat,     // Character combat missions
        Trade,      // Trading missions
        Escort,     // Protect ships/characters
        Exploration // Discover locations
    }
    public enum QuestState
    {
        Available,
        InProgress,
        Completed,
        Failed
    }

    public class Quest
    {
        public string Name { get; }
        public string Description { get; }
        public QuestType Type { get; }
        public QuestState State { get; private set; }
        public List<QuestObjective> Objectives { get; }
        public Dictionary<string, int> Rewards { get; } // e.g., "Gold", "Reputation"
        public bool IsCompleted => Objectives.All(o => o.IsCompleted);

        public Quest(string name, string description, QuestType type, Dictionary<string, int> rewards)
        {
            Name = name;
            Description = description;
            Type = type;
            Objectives = new List<QuestObjective>();
            Rewards = rewards;
            State = QuestState.Available;
        }

        public void AddObjective(string description, Action<GameWorld> checkCondition)
        {
            Objectives.Add(new QuestObjective(description, checkCondition));
        }

        public void CheckObjectives(GameWorld gameWorld)
        {
            foreach (var objective in Objectives.Where(o => !o.IsCompleted))
            {
                objective.CheckCondition(gameWorld);
            }
        }
                public void UpdateState(QuestState newState)
        {
            State = newState;
        }
    }

    public interface IQuestObserver
    {
        void OnQuestUpdated(Quest quest, string message);
        void OnQuestStarted(Quest quest);
        void OnQuestCompleted(Quest quest);
        void OnQuestObjectiveUpdated(Quest quest, QuestObjective objective);
    }
}