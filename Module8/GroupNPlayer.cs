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

        public Position GetAttackPosition()
        {
            // If in target mode:
            //   - Use targeting queue
            // Else:
            //   - Get position using search strategy
            // Add to previous attacks
            // Return position
        }

        public void SetAttackResults(List<AttackResult> results)
        {
            // For each result:
            //   - Skip if our grid
            //   - Track hits/misses
            //   - Update target mode if needed
            //   - Update targeting queue
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
    }

}