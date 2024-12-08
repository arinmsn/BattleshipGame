using System;
using System.Collections.Generic;
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
            _previousAttacks.Clear();
            _inTargetMode = false;
            _lastHit = null;
            _targetQueue.Clear();

            int currentRow = 0;
            foreach (var ship in ships._ships)
            {
                try
                {
                    ship.Place(new Position(0, currentRow), Direction.Horizontal);
                    currentRow++;
                }
                catch
                {
                    currentRow++;
                }
            }
        }

        // Simplistic working GetAttackPosition()
        /*
        public Position GetAttackPosition()
        {
            Position pos;
            do
            {
                pos = new Position(_random.Next(_gridSize), _random.Next(_gridSize));
            } while (_previousAttacks.Contains(pos));
            _previousAttacks.Add(pos);
            return pos;
        }
        */

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
            // TODO: Calculate probability for each square
            // Returns dictionary mapping positions to probabilities
            return new Dictionary<Position, double>();
        }

        // Square selection step
        private Position SelectSquareBasedOnProbability(Dictionary<Position, double> probabilities)
        {
            // TODO: Select square based on probability map
            // For now, just do random selection
            Position pos;
            do
            {
                pos = new Position(_random.Next(_gridSize), _random.Next(_gridSize));
            } while (_previousAttacks.Contains(pos));
            return pos;
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

        private bool IsValidPosition(Position pos)
        {
            return pos.X >= 0 && pos.X < _gridSize &&
                   pos.Y >= 0 && pos.Y < _gridSize;
        }
    }
}