using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Invaders.Wpf.Commons;

namespace Invaders.Wpf.Model
{
    public class InvadersModel
    {
        public const string HistoryDataFilePath = "./config/historydata.json";
        public const string HistoryDataDirectory = "./config";
        public const int MaximumPlayerShots = 3;
        public const int InitialStarCount = 50;
        public static readonly Size PlayAreaSize = new Size(400, 300);
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly Random _random = new Random();
        private readonly List<Point> _stars = new List<Point>();
        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown;
        private DateTime _lastUpdated = DateTime.MinValue;

        private Player _player;
        private DateTime? _playerDied;

        private HistoryData _historyData;
        public HistoryData HistoryData => _historyData.Clone();

        public InvadersModel()
        {
            ReadHistoryDataFromFile();
            EndGame();
        }

        public int Score { get; private set; }
        public int Waves { get; private set; }
        public int Lives { get; private set; }
        public bool GameOver { get; private set; }

        public bool PlayerDying => _playerDied.HasValue;

        public void EndGame()
        {
            Log.Debug(nameof(EndGame) + "() called");
            GameOver = true;
            OnGameLost();
            _historyData.UpdateHighestScore(Score);
            WriteHistoryDataFromFile();
        }

        public void StartGame()
        {
            GameOver = false;
            foreach (var invader in _invaders.ToList())
            {
                OnShipChanged(invader, true);
                _invaders.Remove(invader);
            }

            foreach (var shot in _invaderShots.ToList())
            {
                OnShotMoved(shot, true);
                _invaderShots.Remove(shot);
            }

            foreach (var shot in _playerShots.ToList())
            {
                OnShotMoved(shot, true);
                _invaderShots.Remove(shot);
            }

            foreach (var star in _stars.ToList())
            {
                OnStarChanged(star, true);
                _stars.Remove(star);
            }

            _player = new Player();
            OnShipChanged(_player, false);

            _historyData.IncreasePlayedGames(); 

            Score = 0;
            Lives = 2;
            Waves = 0;

            NextWave();
        }

        public void FireShot()
        {
            if (_playerShots.Count < MaximumPlayerShots)
            {
                var location = new Point(_player.Location.X + _player.Area.Width / 2, _player.Location.Y);
                var shot = new Shot(location, Direction.Up);
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
            if (_random.Next(2) == 0 && _stars.Count > InitialStarCount * 0.75)
                RemoveAStar();
            else if (_stars.Count < InitialStarCount * 1.5)
                AddAStar();
        }

        private void AddAStar()
        {
            var point = new Point(_random.Next((int) PlayAreaSize.Width),
                _random.Next(20, (int) PlayAreaSize.Height) - 20);
            if (_stars.Contains(point)) return;
            _stars.Add(point);
            OnStarChanged(point, false);
        }

        private void RemoveAStar()
        {
            if (_stars.Count <= 0) return;
            var starIndex = _random.Next(_stars.Count);
            OnStarChanged(_stars[starIndex], true);
            _stars.RemoveAt(starIndex);
        }

        public void Update(bool paused)
        {
            if (!paused)
            {
                if (!_invaders.Any())
                    NextWave();
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
            OnNextWave();
            Waves++;
            _invaders.Clear();
            for (var row = 0; row <= 5; row++)
            for (var column = 0; column < 11; column++)
            {
                var location = new Point(column * Invader.InvaderSize.Width * 1.4,
                    row * Invader.InvaderSize.Height * 1.4);
                Invader invader;
                switch (row)
                {
                    case 0:
                        invader = new Invader(location, InvaderType.Spaceship);
                        break;
                    case 1:
                        invader = new Invader(location, InvaderType.Bug);
                        break;
                    case 2:
                        invader = new Invader(location, InvaderType.Saucer);
                        break;
                    case 3:
                        invader = new Invader(location, InvaderType.Satellite);
                        break;
                    case 4:
                        invader = new Invader(location, InvaderType.Star);
                        break;
                    case 5:
                        invader = new Invader(location, InvaderType.Star);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(row));
                }

                _invaders.Add(invader);
                OnShipChanged(invader, false);
            }
        }

        private void CheckForPlayerCollisions()
        {
            var removeAllShots = false;
            var result =
                from invader in _invaders
                where invader.Area.Bottom > _player.Area.Top + _player.Size.Height
                select invader;
            if (result.Any()) EndGame();

            var shotsHit =
                from shot in _invaderShots
                where _player.Area.Contains(shot.Location)
                select shot;
            if (shotsHit.Any())
            {
                Lives--;
                if (Lives == 0)
                {
                    // Game over!
                    EndGame();
                }
                else
                {   
                    // Player died but game is not over!
                    _historyData.IncreaseDiedTime();
                    _playerDied = DateTime.Now;
                    OnShipChanged(_player, true);
                    removeAllShots = true;
                }
            }

            if (removeAllShots)
                foreach (var shot in _invaderShots.ToList())
                {
                    _invaderShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
        }

        private void CheckForInvaderCollisions()
        {
            var hitShots = new List<Shot>();
            var shotInvaders = new List<Invader>();
            foreach (var shot in _playerShots)
            {
                var result =
                    from invader in _invaders
                    where invader.Area.Contains(shot.Location)
                    select new {KilledInvader = invader, HitShot = shot};
                if (result.ToList().Any())
                    foreach (var each in result.ToList())
                    {
                        hitShots.Add(each.HitShot);
                        shotInvaders.Add(each.KilledInvader);
                    }
            }

            foreach (var deadInvader in shotInvaders)
            {
                Score += deadInvader.Score;
                _invaders.Remove(deadInvader);
                _historyData.IncreaseKilledInvaders();
                OnShipChanged(deadInvader, true);
            }

            foreach (var usedShot in hitShots)
            {
                _playerShots.Remove(usedShot);
                OnShotMoved(usedShot, true);
            }
        }

        private void MoveInvaders()
        {
            var timeGap = Math.Min(10 - Waves, 1) * 2 * _invaders.Count;
            if (DateTime.Now - _lastUpdated > TimeSpan.FromMilliseconds(timeGap))
            {
                _lastUpdated = DateTime.Now;

                var invadersTouchingLeft =
                    from invader in _invaders
                    where invader.Area.Left < Invader.HorizontalInterval
                    select invader;
                var invadersTouchingRight =
                    from invader in _invaders
                    where invader.Area.Right > PlayAreaSize.Width - Invader.HorizontalInterval * 2
                    select invader;

                if (!_justMovedDown)
                {
                    if (invadersTouchingLeft.Any())
                    {
                        foreach (var invader in _invaders)
                        {
                            invader.Move(Direction.Down);
                            OnShipChanged(invader, false);
                        }

                        _invaderDirection = Direction.Right;
                    }
                    else if (invadersTouchingRight.Any())
                    {
                        foreach (var invader in _invaders)
                        {
                            invader.Move(Direction.Down);
                            OnShipChanged(invader, false);
                        }

                        _invaderDirection = Direction.Left;
                    }

                    _justMovedDown = true;
                }
                else
                {
                    _justMovedDown = false;
                    foreach (var invader in _invaders)
                    {
                        invader.Move(_invaderDirection);
                        OnShipChanged(invader, false);
                    }
                }
            }
        }

        private void MoveShots()
        {
            foreach (var shot in _playerShots)
            {
                shot.Move();
                OnShotMoved(shot, false);
            }

            foreach (var shot in _invaderShots)
            {
                shot.Move();
                OnShotMoved(shot, false);
            }

            var outOfBoundPlayerShots =
                from playerShot in _playerShots
                where playerShot.Location.Y < 10
                select playerShot;
            var outOfBoundInvaderShots =
                from invaderShot in _invaderShots
                where invaderShot.Location.Y > PlayAreaSize.Height - 10
                select invaderShot;

            foreach (var shot in outOfBoundInvaderShots.ToList())
            {
                _invaderShots.Remove(shot);
                OnShotMoved(shot, true);
            }

            foreach (var shot in outOfBoundPlayerShots.ToList())
            {
                _playerShots.Remove(shot);
                OnShotMoved(shot, true);
            }
        }

        private void ReturnFire()
        {
            if (!_invaders.Any()) return;
            var invaderShots =
                from shot in _invaderShots
                select shot;
            if (invaderShots.Count() > Waves + 1 || _random.Next(10) < 10 - Waves) return;
            var avaliableInvaderGroups =
                from invader in _invaders
                group invader by invader.Location.X
                into invaderGroup
                orderby invaderGroup.Key descending
                select invaderGroup;

            var randomGroup = avaliableInvaderGroups.ElementAt(_random.Next(avaliableInvaderGroups.ToList().Count));
            var bottomInvader = randomGroup.Last();

            var shotLocation = new Point(bottomInvader.Area.X + bottomInvader.Area.Width / 2,
                bottomInvader.Area.Bottom + 2);
            var newShot = new Shot(shotLocation, Direction.Down);
            _invaderShots.Add(newShot);
            OnShotMoved(newShot, false);
        }

        private void ReadHistoryDataFromFile()
        {
            if (!Directory.Exists(HistoryDataDirectory))
            {
                Directory.CreateDirectory(HistoryDataDirectory);
            }
            if (!File.Exists(HistoryDataFilePath))
            {
                _historyData = new HistoryData();
                return;
            }
            using (Stream reader = File.OpenRead(HistoryDataFilePath))
            {
                var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                var obj = serializer.ReadObject(reader);
                if (obj is HistoryData)
                {
                    _historyData = obj as HistoryData;
                } else {
                    throw new SerializationException(nameof(obj) + " is not " + nameof(HistoryData));
                }
            }
        }

        private void WriteHistoryDataFromFile()
        {
            if (_historyData == null) { throw new SerializationException(nameof(_historyData) + " is null."); }
            if (!File.Exists(HistoryDataFilePath))
            {
                using (Stream creater = File.Create(HistoryDataFilePath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                    serializer.WriteObject(creater, _historyData);
                }
            } else {
                using (Stream writer = File.OpenWrite(HistoryDataFilePath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                    serializer.WriteObject(writer, _historyData);
                }
            }
        }

        public event EventHandler<StarChangedEventArgs> StarChanged;

        private void OnStarChanged(Point point, bool disappeared)
        {
            var starChanged = StarChanged;
            starChanged?.Invoke(this, new StarChangedEventArgs(point, disappeared));
        }

        public event EventHandler<ShipChangedEventArgs> ShipChanged;

        private void OnShipChanged(Ship ship, bool killed)
        {
            var shipChanged = ShipChanged;
            shipChanged?.Invoke(this, new ShipChangedEventArgs(ship, killed));
        }

        public event EventHandler<ShotMovedEventArgs> ShotMoved;

        private void OnShotMoved(Shot shot, bool disappeared)
        {
            var shotMoved = ShotMoved;
            shotMoved?.Invoke(this, new ShotMovedEventArgs(shot, disappeared));
        }

        public event EventHandler NextWaveGenerated;

        private void OnNextWave()
        {
            var nextWave = NextWaveGenerated;
            nextWave?.Invoke(this, new EventArgs());
        }

        public event EventHandler GameLost;

        private void OnGameLost()
        {
            var gameLost = GameLost;
            gameLost?.Invoke(this, new EventArgs());
        }
    }
}