using System;
using System.Collections.Generic;
using Module8;

namespace CS3110.Module8.Group1
{
    public class GroupNPlayer : IPlayer
    {
        // Fields/Properties
        private string _name;
        private int _playerIndex;
        private int _gridSize;
        private Random _random;
        private List<Position> _previousAttacks;
        private Dictionary<int, List<Position>> _hits;
        private bool _inTargetMode;
        private Position _lastHit;
        private List<Position> _targetQueue;

        // Constructor
        public GroupNPlayer(string name)
        {
            _name = name;
            _random = new Random();
            _previousAttacks = new List<Position>();
            _hits = new Dictionary<int, List<Position>>();
            _targetQueue = new List<Position>();
        }

        // Interface Properties
        public string Name => _name;
        public int Index => _playerIndex;

        // Interface Methods - Pseudocode for now
        public void StartNewGame(int playerIndex, int gridSize, Ships ships)
        {
            // Store player index and grid size
            // Reset all tracking lists
            // Place ships on grid:
            //   - Loop through each ship
            //   - Find valid position
            //   - Set ship position
        }

        public void SetAttackResults(List<AttackResult> results)
        {
            // For each result:
            //   - Skip if our grid
            //   - Track hits/misses
            //   - Update target mode if needed
            //   - Update targeting queue
        }
        // GetAttackPosition needs to be divided into these components:
        // 1. Probability calculation
        private Dictionary<Position, double> CalculateShipProbabilities()
        {
            // TODO: Calculate probability for each square
            // Returns dictionary mapping positions to probabilities
            return new Dictionary<Position, double>();
        }

        // 2. Square selection based on probabilities
        private Position SelectSquareBasedOnProbability(Dictionary<Position, double> probabilities)
        {
            // TODO: Select square based on probability map
            // For now, return random valid position to avoid crashing
            return GetRandomPosition();
        }

        // 3. Mode-based selection (search vs target mode)
        public Position GetAttackPosition()
        {
            // For now, just return random position to avoid crashing
            return GetRandomPosition();
        }

        // Helper Methods
        private bool IsValidPosition(Position pos)
        {
            // Check if position is within grid
        }

        private bool HasAttacked(Position pos)
        {
            // Check if position was previously attacked
        }

        private void AddAdjacentPositions(Position pos)
        {
            // Add valid adjacent positions to target queue
        }

        private Position GetRandomPosition()
        {
            Position pos;
            do
            {
                pos = new Position
                (
                    _random.Next(_gridSize),
                    _random.Next(_gridSize)
                );
            } while (_previousAttacks.Contains(pos));
            return pos;
        }
    }

}