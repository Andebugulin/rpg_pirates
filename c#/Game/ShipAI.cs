namespace Game{
    public class ShipAI
    {
        private Ship controlledShip;
        private Random rng = new Random();
        private int movementCooldown = 0;
        
        public ShipAI(Ship ship)
        {
            controlledShip = ship;
        }
        
        public void UpdateBehavior(GameWorld gameWorld)
        {
            if (movementCooldown > 0)
            {
                movementCooldown--;
                return;
            }
            
            // Get player ship position
            Position playerPos = gameWorld.playerShip.Position;
            
            // Calculate distance to player
            int distanceToPlayer = Math.Abs(controlledShip.Position.X - playerPos.X) + 
                                Math.Abs(controlledShip.Position.Y - playerPos.Y);
            
            // Decide behavior based on distance and random chance
            if (distanceToPlayer <= 3 && rng.Next(100) < 30) // 30% chance to attack if close
            {
                MoveTowardsPlayer(gameWorld);
            }
            else if (distanceToPlayer <= 2) // Run away if too close
            {
                MoveAwayFromPlayer(gameWorld);
            }
            else // Random movement
            {
                MoveRandomly(gameWorld);
            }
            
            movementCooldown = rng.Next(2, 5); // Wait 2-4 turns before next move
        }
        
        private void MoveTowardsPlayer(GameWorld gameWorld)
        {
            Position playerPos = gameWorld.playerShip.Position;
            Position newPos = new Position(controlledShip.Position.X, controlledShip.Position.Y);
            
            if (playerPos.X > controlledShip.Position.X) newPos.X++;
            else if (playerPos.X < controlledShip.Position.X) newPos.X--;
            
            if (playerPos.Y > controlledShip.Position.Y) newPos.Y++;
            else if (playerPos.Y < controlledShip.Position.Y) newPos.Y--;
            
            if (IsValidMove(newPos, gameWorld))
                gameWorld.MoveEntity(controlledShip, newPos);
        }
        
        private void MoveAwayFromPlayer(GameWorld gameWorld)
        {
            Position playerPos = gameWorld.playerShip.Position;
            Position newPos = new Position(controlledShip.Position.X, controlledShip.Position.Y);
            
            if (playerPos.X > controlledShip.Position.X) newPos.X--;
            else if (playerPos.X < controlledShip.Position.X) newPos.X++;
            
            if (playerPos.Y > controlledShip.Position.Y) newPos.Y--;
            else if (playerPos.Y < controlledShip.Position.Y) newPos.Y++;
            
            if (IsValidMove(newPos, gameWorld))
                gameWorld.MoveEntity(controlledShip, newPos);
        }
        
        private void MoveRandomly(GameWorld gameWorld)
        {
            int direction = rng.Next(4);
            Position newPos = new Position(controlledShip.Position.X, controlledShip.Position.Y);
            
            switch (direction)
            {
                case 0: newPos.Y--; break; // Up
                case 1: newPos.Y++; break; // Down
                case 2: newPos.X--; break; // Left
                case 3: newPos.X++; break; // Right
            }
            
            if (IsValidMove(newPos, gameWorld))
                gameWorld.MoveEntity(controlledShip, newPos);
        }
        
        private bool IsValidMove(Position pos, GameWorld gameWorld)
        {
            return pos.X >= 0 && pos.X < gameWorld.width && 
                pos.Y >= 0 && pos.Y < gameWorld.height;
        }
    }
}