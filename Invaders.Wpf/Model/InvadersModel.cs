using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class InvadersModel
    {
        public static readonly Size PlayAreaSize = new Size(400, 300);
        public const int MaximumPlayerShots = 3;
        public const int InitialStarCount = 50;
        private readonly Random _random = new Random();
        public int Score { get; private set; }
        public int Waves { get; private set; }
        public int Lives { get; private set; }
        public bool GameOver { get; private set; }
        private DateTime? _playerDied;

        public bool PlayerDying => _playerDied.HasValue;

        private Player _player;
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();
        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown = false;
        private DateTime _lastUpdated = DateTime.MinValue;

        public InvadersModel()
        {
            EndGame();
        }

        public void EndGame()
        {
            GameOver = true;
        }

        public void StartGame()
        {
            GameOver = false;
            foreach (var invader in _invaders.ToList())
            {
                OnShipChanged(invader, true);
                _invaders.Remove(invader);
            }

            foreach (var shot in _invaderShots)
            {
                OnShotMoved(shot, true);
                _invaderShots.Remove(shot);
            }

            foreach (var shot in _playerShots)
            {
                OnShotMoved(shot, true);
                _invaderShots.Remove(shot);
            }

            foreach (var star in _stars)
            {
                OnStarChanged(star, true);
                _stars.Remove(star);
            }
            _player = new Player();
            OnShipChanged(_player, false);
            
            Lives = 2;
            Waves = 0;
            
            NextWave();
        }

        public void FireShot()
        {
            if (_playerShots.Count < 3)
            {
                Point location = new Point((_player.Area.Right - _player.Area.Left) / 2, _player.Area.Top);
                Shot shot = new Shot(location, Direction.Up);
                _playerShots.Add(shot);
                OnShotMoved(shot, false);
            }
        }

        public void MovePlayer(Direction direction)
        {
            if (!PlayerDying)
            {
                _player.Move(direction);
                OnShipChanged(_player, false);
            }
        }

        public void Twinkle()
        {
            throw new NotImplementedException();
        }

        public void Update(bool paused)
        {
            if (!paused)
            {
                if (!_invaders.Any()) NextWave();
                if (!PlayerDying)
                {
                    MoveInvaders();
                    MoveShots();
                    ReturnFire();
                    CheckForInvaderCollisions();
                    CheckForPlayerCollisions();
                }
                else if (DateTime.Now - _playerDied.Value > TimeSpan.FromSeconds(2.5))
                {
                    _playerDied = null;
                    OnShipChanged(_player, false);
                }
            }
            Twinkle();
        }
        
        public void UpdateAllShipsAndStars()
        {
            foreach (var invader in _invaders) OnShipChanged(invader, false);
            foreach (var shot in _invaderShots) OnShotMoved(shot, false);
            foreach (var shot in _playerShots) OnShotMoved(shot, false);
            OnShipChanged(_player, false);
            foreach (var star in _stars) OnStarChanged(star, false);
        }

        private void NextWave()
        {
            throw new NotImplementedException();
        }

        private void CheckForPlayerCollisions()
        {
            bool removeAllShots = false;
            var result = 
                from invader in _invaders 
                where invader.Area.Bottom > _player.Area.Top + _player.Size.Height 
                select invader;
            if (result.Any())
            {
                EndGame();
            }

            var shotsHit =
                from shot in _invaderShots
                where _player.Area.Contains(shot.Location)
                select shot;
            if (shotsHit.Any())
            {
                Lives--;
                if (Lives == 0)
                {
                    EndGame();
                }
                else
                {
                    _playerDied = DateTime.Now;
                    OnShipChanged(_player, true);
                    removeAllShots = true;
                }
            }

            if (removeAllShots)
            {
                foreach (var shot in _invaderShots.ToList())
                {
                    _invaderShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
            }
        }

        private void CheckForInvaderCollisions()
        {
            throw new NotImplementedException();
        }

        private void MoveInvaders()
        {
            _invaders.Clear();
            //TODO: Add Invader objects, aets their Location attrs, make them in order.
        }

        private void MoveShots()
        {
            foreach (var shot in _playerShots) { shot.Move(); OnShotMoved(shot, false); }  
            foreach (var shot in _invaderShots) { shot.Move(); OnShotMoved(shot, false); }

            var outOfBoundPlayerShots =
                from playerShot in _playerShots
                where playerShot.Location.Y > PlayAreaSize.Height - 10
                select playerShot;
            var outOfBoundInvaderShots =
                from invaderShot in _invaderShots
                where invaderShot.Location.Y < 10
                select invaderShot;
            
            foreach (var shot in outOfBoundInvaderShots) { _invaderShots.Remove(shot); OnShotMoved(shot, true); }
            foreach (var shot in outOfBoundPlayerShots) {_playerShots.Remove(shot); OnShotMoved(shot, true); }
        }
        
        private void ReturnFire()
        {
            throw new NotImplementedException();
        }
        
        public event EventHandler<StarChangedEventArgs> StarChanged;

        private void OnStarChanged(Point point, bool disappeared)
        {
            EventHandler<StarChangedEventArgs> starChanged = StarChanged;
            starChanged?.Invoke(this, new StarChangedEventArgs(point, disappeared));
        }

        public event EventHandler<ShipChangedEventArgs> ShipChanged;

        private void OnShipChanged(Ship ship, bool killed)
        {
            EventHandler<ShipChangedEventArgs> shipChanged = ShipChanged;
            shipChanged?.Invoke(this, new ShipChangedEventArgs(ship, killed));
        }

        public event EventHandler<ShotMovedEventArgs> ShotMoved;

        private void OnShotMoved(Shot shot, bool disappeared)
        {
            EventHandler<ShotMovedEventArgs> shotMoved = ShotMoved;
            shotMoved?.Invoke(this, new ShotMovedEventArgs(shot, disappeared));
        }
    }
}