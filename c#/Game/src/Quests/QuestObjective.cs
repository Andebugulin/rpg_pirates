namespace Game
{
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
}