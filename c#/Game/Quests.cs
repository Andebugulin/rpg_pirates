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

    public class QuestObjective
    {
        public string Description { get; }
        public bool IsCompleted { get; private set; }
        public Action<GameWorld> CheckCondition { get; }

        public QuestObjective(string description, Action<GameWorld> checkCondition)
        {
            Description = description;
            CheckCondition = checkCondition;
            IsCompleted = false;
        }

        public void Complete()
        {
            IsCompleted = true;
        }
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

   public class QuestManager
{
    private readonly List<Quest> activeQuests = new();
    private readonly List<IQuestObserver> observers = new();
    private readonly Dictionary<Quest, HashSet<IQuestObserver>> questSpecificObservers = new();
    private bool isNotifying = false; // Guard flag to prevent recursive registration

    // Register observer for all quests
    public void RegisterObserver(IQuestObserver observer)
    {
        if (observers.Contains(observer))
            return;
            
        observers.Add(observer);
        
        // Only notify about existing quests if we're not already in a notification cycle
        if (!isNotifying)
        {
            try
            {
                isNotifying = true;
                foreach (var quest in activeQuests)
                {
                    observer.OnQuestStarted(quest);
                }
            }
            finally
            {
                isNotifying = false;
            }
        }
    }

    // Register observer for specific quest
    public void RegisterObserverForQuest(IQuestObserver observer, Quest quest)
    {
        var observers = questSpecificObservers.GetOrAdd(quest, _ => new HashSet<IQuestObserver>());
        
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            
            // Only notify about the quest if we're not already in a notification cycle
            if (!isNotifying)
            {
                try
                {
                    isNotifying = true;
                    observer.OnQuestStarted(quest);
                }
                finally
                {
                    isNotifying = false;
                }
            }
        }
    }

    public void UnregisterObserver(IQuestObserver observer)
    {
        observers.Remove(observer);
        foreach (var observerSet in questSpecificObservers.Values)
        {
            observerSet.Remove(observer);
        }
    }

    public void UnregisterObserverFromQuest(IQuestObserver observer, Quest quest)
    {
        questSpecificObservers.GetValueOrDefault(quest)?.Remove(observer);
    }

    private void NotifyObservers(Quest quest, string message)
    {
        if (isNotifying)
            return;

        try
        {
            isNotifying = true;
            
            // Notify global observers
            foreach (var observer in observers.ToList())
            {
                observer.OnQuestUpdated(quest, message);
            }

            // Notify quest-specific observers
            if (questSpecificObservers.TryGetValue(quest, out var specificObservers))
            {
                foreach (var observer in specificObservers.ToList())
                {
                    observer.OnQuestUpdated(quest, message);
                }
            }
        }
        finally
        {
            isNotifying = false;
        }
    }

    private void NotifyQuestCompleted(Quest quest)
    {
        if (isNotifying)
            return;

        try
        {
            isNotifying = true;
            
            var allObservers = observers
                .Concat(questSpecificObservers.GetValueOrDefault(quest) ?? Enumerable.Empty<IQuestObserver>())
                .ToList();  // Create a snapshot of observers
                
            foreach (var observer in allObservers)
            {
                observer.OnQuestCompleted(quest);
            }
        }
        finally
        {
            isNotifying = false;
        }
    }

    private void NotifyObjectiveUpdated(Quest quest, QuestObjective objective)
    {
        if (isNotifying)
            return;

        try
        {
            isNotifying = true;
            
            var allObservers = observers
                .Concat(questSpecificObservers.GetValueOrDefault(quest) ?? Enumerable.Empty<IQuestObserver>())
                .ToList();  // Create a snapshot of observers
                
            foreach (var observer in allObservers)
            {
                observer.OnQuestObjectiveUpdated(quest, objective);
            }
        }
        finally
        {
            isNotifying = false;
        }
    }

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        NotifyObservers(quest, $"New quest available: {quest.Name}");
    }

    public void UpdateQuests(GameWorld gameWorld)
    {
        foreach (var quest in activeQuests.Where(q => !q.IsCompleted))
        {
            var completedBefore = quest.Objectives.Count(o => o.IsCompleted);
            quest.CheckObjectives(gameWorld);
            var completedAfter = quest.Objectives.Count(o => o.IsCompleted);

            if (completedBefore != completedAfter)
            {
                // Find and notify about newly completed objectives
                var newlyCompleted = quest.Objectives
                    .Where(o => o.IsCompleted)
                    .Skip(completedBefore);
                    
                foreach (var objective in newlyCompleted)
                {
                    NotifyObjectiveUpdated(quest, objective);
                }

                NotifyObservers(quest, $"Quest progress updated: {quest.Name}");
            }

            if (quest.IsCompleted)
            {
                quest.UpdateState(QuestState.Completed);
                NotifyQuestCompleted(quest);
            }
        }
    }

    public void FailQuest(Quest quest)
    {
        if (activeQuests.Contains(quest))
        {
            quest.UpdateState(QuestState.Failed);
            NotifyObservers(quest, $"Quest failed: {quest.Name}");
        }
    }
}
    // Quest Factory
    public class QuestFactory
    {
        public static Quest CreateShipBattleQuest(string targetShipName)
        {
            var rewards = new Dictionary<string, int>
            {
                { "Gold", 1000 },
                { "Reputation", 50 }
            };

            var quest = new Quest(
                $"Defeat the {targetShipName}",
                $"Find and defeat the ship named {targetShipName}.",
                QuestType.Combat,
                rewards
            );

            quest.AddObjective($"Locate the {targetShipName}", (gameWorld) => {
            });

            quest.AddObjective($"Defeat {targetShipName}", (gameWorld) => {
            });

            return quest;
        }

        public static Quest CreatePirateHuntQuest(int pirateCount)
        {
            var rewards = new Dictionary<string, int>
            {
                { "Gold", 500 * pirateCount },
                { "Reputation", 20 * pirateCount }
            };

            var quest = new Quest(
                "Pirate Hunt",
                $"Defeat {pirateCount} pirates terrorizing the seas.",
                QuestType.Combat,
                rewards
            );

            quest.AddObjective($"Defeat {pirateCount} pirates", (gameWorld) => {
            });

            return quest;
        }

    }
}