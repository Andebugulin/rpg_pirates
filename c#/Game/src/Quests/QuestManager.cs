namespace Game
{
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
}