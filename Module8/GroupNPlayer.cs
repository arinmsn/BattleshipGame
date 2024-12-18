﻿using System;
using System.Collections.Generic;
using System.Linq;
using Module8;

namespace CS3110.Module8.Group1
{
    public class GroupNPlayer : IPlayer
    {
        private string _name;
        private int _playerIndex;
        private int _gridSize;
        private Random _random;
        private List<Position> _previousAttacks;
        private bool _inTargetMode;
        private Position _lastHit;
        private List<Position> _targetQueue;
        private Ships _ships;

        public GroupNPlayer(string name)
        {
            _name = name;
            _random = new Random();
            _previousAttacks = new List<Position>();
            _targetQueue = new List<Position>();
        }

        public string Name => _name;
        public int Index => _playerIndex;

        public void StartNewGame(int playerIndex, int gridSize, Ships ships)
        {
            _playerIndex = playerIndex;
            _gridSize = gridSize;
            _ships = ships;
            _previousAttacks.Clear();
            _inTargetMode = false;
            _lastHit = null;
            _targetQueue.Clear();

            int currentRow = 0;
            foreach (var ship in ships._ships)
            {
                try
                {
                    // Ensure we don't exceed grid size
                    if (currentRow >= _gridSize)
                        currentRow = 0;

                    ship.Place(new Position(0, currentRow), Direction.Horizontal);
                    currentRow++;
                }
                catch
                {
                    if (currentRow >= _gridSize)
                        currentRow = 0;
                    else
                        currentRow++;
                }
            }
        }

        public void SetAttackResults(List<AttackResult> results)
        {
            foreach (var result in results)
            {
                if (result.PlayerIndex == _playerIndex)
                    continue;

                if (result.ResultType == AttackResultType.Hit)
                {
                    _inTargetMode = true;
                    _lastHit = result.Position;
                    AddAdjacentPositions(result.Position);
                }
                else if (result.ResultType == AttackResultType.Sank)
                {
                    _targetQueue.Clear();
                    _inTargetMode = false;
                }
            }
        }

        // Probability calculation step
        private Dictionary<Position, double> CalculateShipProbabilities()
        {
            var probabilities = new Dictionary<Position, double>();

            // Initialize probabilities for each grid position
            for (int x = 0; x < _gridSize; x++)
            {
                for (int y = 0; y < _gridSize; y++)
                {
                    var pos = new Position(x, y);
                    if (!_previousAttacks.Contains(pos))
                    {
                        probabilities[pos] = 0.0;
                    }
                }
            }

            // Analyze each ship's potential placement
            foreach (var ship in _ships.GetRemainingShips())
            {
                int shipLength = ship.Size;

                // Horizontal placement
                for (int x = 0; x <= _gridSize - shipLength; x++)
                {
                    for (int y = 0; y < _gridSize; y++)
                    {
                        bool canPlace = true;
                        var positions = new List<Position>();

                        for (int offset = 0; offset < shipLength; offset++)
                        {
                            var pos = new Position(x + offset, y);
                            if (_previousAttacks.Contains(pos))
                            {
                                canPlace = false;
                                break;
                            }
                            positions.Add(pos);
                        }

                        if (canPlace)
                        {
                            foreach (var pos in positions)
                            {
                                if (probabilities.ContainsKey(pos))
                                {
                                    probabilities[pos] += 1.0;
                                }
                            }
                        }
                    }
                }

                // Vertical placement
                for (int x = 0; x < _gridSize; x++)
                {
                    for (int y = 0; y <= _gridSize - shipLength; y++)
                    {
                        bool canPlace = true;
                        var positions = new List<Position>();

                        for (int offset = 0; offset < shipLength; offset++)
                        {
                            var pos = new Position(x, y + offset);
                            if (_previousAttacks.Contains(pos))
                            {
                                canPlace = false;
                                break;
                            }
                            positions.Add(pos);
                        }

                        if (canPlace)
                        {
                            foreach (var pos in positions)
                            {
                                if (probabilities.ContainsKey(pos))
                                {
                                    probabilities[pos] += 1.0;
                                }
                            }
                        }
                    }
                }
            }

            // Normalize probabilities
            double maxProbability = probabilities.Values.Max();
            if (maxProbability > 0)
            {
                foreach (var key in probabilities.Keys.ToList())
                {
                    probabilities[key] /= maxProbability;
                }
            }

            return probabilities;
        }


        // Square selection step
        private Position SelectSquareBasedOnProbability(Dictionary<Position, double> probabilities)
        {
            // Use systematic scanning for first moves 
            if (_previousAttacks.Count < _gridSize * 3)  // Increased from 2 to 3 for more systematic early scanning
            {
                // Modified to prefer corners and edges first
                for (int y = 0; y < _gridSize; y++)
                {
                    for (int x = 0; x < _gridSize; x++)
                    {
                        var pos = new Position(x, y);
                        // Check corners and edges first
                        bool isCornerOrEdge = x == 0 || x == _gridSize - 1 || y == 0 || y == _gridSize - 1;
                        if (!_previousAttacks.Contains(pos) && isCornerOrEdge)
                            return pos;
                    }
                }
                // Then check remaining positions
                for (int y = 0; y < _gridSize; y++)
                {
                    for (int x = 0; x < _gridSize; x++)
                    {
                        var pos = new Position(x, y);
                        if (!_previousAttacks.Contains(pos))
                            return pos;
                    }
                }
            }

            // Then use probability-based targeting for later moves
            var topPositions = probabilities
                .OrderByDescending(kvp => kvp.Value)
                .Take(4)  // Increased from 3 to 4 for more variety
                .ToList();

            if (!topPositions.Any())
            {
                Position pos;
                do
                {
                    pos = new Position(_random.Next(_gridSize), _random.Next(_gridSize));
                } while (_previousAttacks.Contains(pos));
                return pos;
            }

            // Modified probability distribution
            double randomValue = _random.NextDouble();
            if (randomValue < 0.7 || topPositions.Count == 1)  // Reduced from 0.8 to 0.7
                return topPositions[0].Key;
            else if (randomValue < 0.9 && topPositions.Count >= 2)  // Increased middle range
                return topPositions[1].Key;
            else if (randomValue < 0.95 && topPositions.Count >= 3)
                return topPositions[2].Key;
            else if (topPositions.Count >= 4)
                return topPositions[3].Key;

            return topPositions[0].Key;
        }

        // We can also modify GetAttackPosition to use our targeting mode logic
        public Position GetAttackPosition()
        {
            Position pos;

            // If we're in target mode and have positions to try
            if (_inTargetMode && _targetQueue.Count > 0)
            {
                pos = _targetQueue[0];
                _targetQueue.RemoveAt(0);
            }
            else
            {
                // Use probability-based selection (currently just returns random)
                var probabilities = CalculateShipProbabilities();
                pos = SelectSquareBasedOnProbability(probabilities);
            }

            // Add to previous attacks and return
            if (!_previousAttacks.Contains(pos))
            {
                _previousAttacks.Add(pos);
            }
            return pos;
        }

        private void AddAdjacentPositions(Position pos)
        {
            // Clear existing queue to focus on most recent hit
            _targetQueue.Clear();

            // If we have a previous hit, prioritize positions in that direction
            if (_lastHit != null && _lastHit != pos)
            {
                int dx = pos.X - _lastHit.X;
                int dy = pos.Y - _lastHit.Y;

                // Continue in the same direction
                var nextPos = new Position(pos.X + dx, pos.Y + dy);
                if (IsValidPosition(nextPos) && !_previousAttacks.Contains(nextPos))
                    _targetQueue.Add(nextPos);

                // Try opposite direction from first hit
                var oppositePos = new Position(_lastHit.X - dx, _lastHit.Y - dy);
                if (IsValidPosition(oppositePos) && !_previousAttacks.Contains(oppositePos))
                    _targetQueue.Add(oppositePos);
            }

            // Add standard adjacent positions if queue is empty
            if (_targetQueue.Count == 0)
            {
                var adjacentPositions = new List<Position>
        {
            new Position(pos.X - 1, pos.Y),
            new Position(pos.X + 1, pos.Y),
            new Position(pos.X, pos.Y - 1),
            new Position(pos.X, pos.Y + 1)
        };

                foreach (var adjPos in adjacentPositions)
                {
                    if (IsValidPosition(adjPos) && !_previousAttacks.Contains(adjPos))
                    {
                        _targetQueue.Add(adjPos);
                    }
                }
            }
        }

        private bool IsValidPosition(Position pos)
        {
            return pos.X >= 0 && pos.X < _gridSize &&
                   pos.Y >= 0 && pos.Y < _gridSize;
        }
    }
}