namespace Game
{
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

            quest.AddObjective($"Defeat {pirateCount} pirates", (gameWorld) => 
            {
                int totalKilledEnemies = 0;
                foreach (var crewMember in GameWorld.Instance.playerShip.Crew)
                {
                    totalKilledEnemies += crewMember.KilledEnemies;
                }

                if (totalKilledEnemies >= pirateCount)
                {
                    quest.Objectives[0].Complete();
                }
            });

            return quest;
        }

    }
}