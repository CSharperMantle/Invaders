using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using Invaders.Wpf.Commons;

namespace Invaders.Wpf.Model
{
    public class InvadersModel
    {
        public const string HistoryDataFilePath = "./config/historydata.json";
        public const string HistoryDataDirectory = "./config";
        public const string UserSettingsFilePath = "./config/usersettings.json";
        public const string UserSettingsDirectory = "./config";
        public const int MaximumPlayerShots = 3;
        public const int InitialStarCount = 25;
        public const int InitialLives = 3;
        public static readonly Size PlayAreaSize = new Size(400, 300);
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly Random _random = new Random();
        private readonly List<Point> _stars = new List<Point>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private HistoryData _historyData;
        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown;
        private DateTime _lastUpdated = DateTime.MinValue;

        private Player _player;
        private DateTime? _playerDied;
        private UserSettings _userSettings;

        public InvadersModel()
        {
            ReadHistoryDataFromFile();
            EndGame();
        }

        public HistoryData HistoryData => _historyData.Clone();

        public int Score { get; private set; }
        public int Waves { get; private set; }
        public int Lives { get; private set; }
        public bool GameOver { get; private set; }
        public ShotType CurrentShotType { get; set; }

        public bool PlayerDying => _playerDied.HasValue;

        public void EndGame()
        {
            Log.D(nameof(EndGame) + "() called");
            GameOver = true;
            OnGameLost();
            _stopwatch.Stop();
            _historyData.UpdateHighestScore(Score);
            _historyData.IncreasePlayedTime(_stopwatch.Elapsed);
            _stopwatch.Reset();
            WriteHistoryDataToFile();
        }

        public void StartGame()
        {
            GameOver = false;
            foreach (var invader in _invaders)
            {
                OnShipChanged(invader, true);
            }
            _invaders.Clear();

            foreach (var shot in _invaderShots)
            {
                OnShotMoved(shot, true);
            }
            _invaderShots.Clear();

            foreach (var shot in _playerShots)
            {
                OnShotMoved(shot, true);
            }
            _playerShots.Clear();

            foreach (var star in _stars)
            {
                OnStarChanged(star, true);
            }
            _stars.Clear();

            _player = new Player();
            OnShipChanged(_player, false);

            _historyData.IncreasePlayedGames();

            Score = 0;
            Lives = InitialLives;
            Waves = 0;
            CurrentShotType = ShotType.BasicShot;

            _stopwatch.Start();

            NextWave();
        }

        public void FireShot()
        {
            if (_playerShots.Count < MaximumPlayerShots)
            {
                var location = new Point(_player.Location.X + _player.Area.Width / 2, _player.Location.Y);
                Shot shot;
                //TODO: Add more kinds of shots
                switch (CurrentShotType)
                {
                    case ShotType.BasicShot:
                        shot = new BasicShot(location, Direction.Up);
                        break;
                    case ShotType.LazerShot:
                        shot = new LazerShot(location, Direction.Up);
                        break;
                    case ShotType.HomingShot:
                        shot = new HomingShot(location, Direction.Up);
                        break;
                    default:
                        throw new ArgumentException(nameof(CurrentShotType));
                }

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

        private void Twinkle()
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
                if (_invaders.Count <= 0)
                    NextWave();
                if (!PlayerDying)
                {
                    MoveInvaders();
                    MoveShots();
                    ReturnFire();
                    CheckForInvaderCollisions();
                    CheckForPlayerCollisions();
                    CheckForShotCollisions();
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
            {
                foreach (var shot in _invaderShots)
                {
                    OnShotMoved(shot, true);
                }
                _invaderShots.Clear();
                foreach (var shot in _playerShots)
                {
                    OnShotMoved(shot, true);
                }
                _playerShots.Clear();
            }
        }

        private void CheckForInvaderCollisions()
        {
            var killedInvaders = new Dictionary<Invader, Shot>();
            foreach (var shot in _playerShots)
            {
                var result =
                    from invader in _invaders
                    where invader.Area.Contains(shot.Location)
                    select new {KilledInvader = invader, HitShot = shot};
                var resultList = result.ToList();
                if (resultList.Count <= 0) continue; // Inverted IF
                foreach (var each in resultList)
                    if (!killedInvaders.ContainsKey(each.KilledInvader))
                    {
                        killedInvaders.Add(each.KilledInvader, each.HitShot);
                    }
                    else
                    {
                        Log.W("Duplicated key found.");
                        Log.W($" at function {new StackTrace().GetFrame(0).GetMethod().Name}");
                    }
            }

            foreach (var killedInvader in killedInvaders.Keys)
            {
                Score += killedInvader.Score;
                _invaders.Remove(killedInvader);
                _historyData.IncreaseKilledInvaders();
                OnShipChanged(killedInvader, true);

                var shot = killedInvaders[killedInvader];
                shot.DecreaseHitpoint(killedInvader.Score);
                if (shot.IsUsedUp())
                {
                    _playerShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
            }
        }

        private void CheckForShotCollisions()
        {
            foreach (var playerShot in _playerShots.ToList())
            foreach (var invaderShot in _invaderShots.ToList())
                if (new Rect(playerShot.Location, playerShot.ShotSize).Contains(invaderShot.Location))
                {
                    OnShotMoved(playerShot, true);
                    OnShotMoved(invaderShot, true);
                    _invaderShots.Remove(invaderShot);
                    _playerShots.Remove(playerShot);
                    Score += invaderShot.Score;
                    _historyData.IncreaseKilledShots();
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
            if (_invaders.Count <= 0) return;
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

            var avaiInvaderGroupsList = avaliableInvaderGroups.ToList();
            var randomGroup = avaiInvaderGroupsList[_random.Next(avaiInvaderGroupsList.Count)];
            var bottomInvader = randomGroup.Last();

            var shotLocation = new Point(bottomInvader.Area.X + bottomInvader.Area.Width / 2,
                bottomInvader.Area.Bottom + 2);
            Shot newShot;
            switch (bottomInvader.InvaderType)
            {
                case InvaderType.Spaceship:
                    newShot = new HomingShot(shotLocation, Direction.Down, _player);
                    break;
                case InvaderType.Bug:
                    newShot = new LazerShot(shotLocation, Direction.Down);
                    break;
                case InvaderType.Saucer:
                    newShot = new LazerShot(shotLocation, Direction.Down);
                    break;
                case InvaderType.Satellite:
                    newShot = new BasicShot(shotLocation, Direction.Down);
                    break;
                case InvaderType.Star:
                    newShot = new BasicShot(shotLocation, Direction.Down);
                    break;
                default:
                    throw new ArgumentException(nameof(bottomInvader));
            }

            _invaderShots.Add(newShot);
            OnShotMoved(newShot, false);
        }

        private void ReadHistoryDataFromFile()
        {
            if (!Directory.Exists(HistoryDataDirectory)) Directory.CreateDirectory(HistoryDataDirectory);
            if (!File.Exists(HistoryDataFilePath))
            {
                _historyData = new HistoryData();
                return;
            }

            using (Stream reader = File.OpenRead(HistoryDataFilePath))
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                    var obj = serializer.ReadObject(reader);
                    if (obj is HistoryData)
                        _historyData = (HistoryData) obj;
                    else
                        throw new SerializationException($"{nameof(obj)} is not {nameof(HistoryData)}");
                }
                catch (Exception e)
                {
                    Log.E(e.ToString());
                    Log.I(e.StackTrace);
                    _historyData = new HistoryData();
                }
            }
        }

        private void ReadUserSettingsFromFile()
        {
            if (!Directory.Exists(UserSettingsDirectory)) Directory.CreateDirectory(UserSettingsDirectory);
            if (!File.Exists(UserSettingsFilePath))
            {
                _userSettings = new UserSettings();
                return;
            }

            using (Stream reader = File.OpenRead(UserSettingsFilePath))
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(UserSettings));
                    var obj = serializer.ReadObject(reader);
                    if (obj is UserSettings)
                        _userSettings = (UserSettings) obj;
                    else
                        throw new SerializationException($"{nameof(obj)} is not {nameof(UserSettings)}");
                }
                catch (Exception e)
                {
                    Log.C(e.ToString());
                    Log.I(e.StackTrace);
                    _userSettings = new UserSettings();
                }
            }
        }

        private void WriteHistoryDataToFile()
        {
            if (_historyData == null) throw new SerializationException($"{nameof(_historyData)} is null.");
            try
            {
                if (!File.Exists(HistoryDataFilePath))
                    using (Stream creater = File.Create(HistoryDataFilePath))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                        serializer.WriteObject(creater, _historyData);
                    }
                else
                    using (Stream writer = File.OpenWrite(HistoryDataFilePath))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(HistoryData));
                        serializer.WriteObject(writer, _historyData);
                    }
            }
            catch (Exception e)
            {
                Log.E(e.ToString());
                Log.I(e.StackTrace);
            }
        }

        private void WriteUserSettingsToFile()
        {
            if (_userSettings == null) throw new SerializationException($"{nameof(_userSettings)} is null.");
            try
            {
                if (!File.Exists(UserSettingsFilePath))
                    using (Stream creater = File.Create(UserSettingsFilePath))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(UserSettings));
                        serializer.WriteObject(creater, _userSettings);
                    }
                else
                    using (Stream writer = File.OpenWrite(HistoryDataFilePath))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(UserSettings));
                        serializer.WriteObject(writer, _userSettings);
                    }
            }
            catch (Exception e)
            {
                Log.E(e.ToString());
                Log.I(e.StackTrace);
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