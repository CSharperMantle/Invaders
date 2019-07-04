using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Invaders.Wpf.Annotations;
using Invaders.Wpf.Model;
using Invaders.Wpf.View;

namespace Invaders.Wpf.ViewModel
{
    public class InvadersViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<Invader, FrameworkElement> _invaders =
            new Dictionary<Invader, FrameworkElement>();

        private readonly ObservableCollection<object> _lives =
            new ObservableCollection<object>();

        private readonly InvadersModel _model = new InvadersModel();

        private readonly List<FrameworkElement> _scanLines =
            new List<FrameworkElement>();

        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders =
            new Dictionary<FrameworkElement, DateTime>();

        private readonly Dictionary<Shot, FrameworkElement> _shots =
            new Dictionary<Shot, FrameworkElement>();

        private readonly ObservableCollection<FrameworkElement> _sprites =
            new ObservableCollection<FrameworkElement>();

        private readonly Dictionary<Point, FrameworkElement> _stars =
            new Dictionary<Point, FrameworkElement>();

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private bool _lastPaused = true;

        private DateTime? _leftAction;
        private FrameworkElement _playerControl;
        private bool _playerFlashing;
        private DateTime? _rightAction;

        public InvadersViewModel()
        {
            Scale = 1;

            _model.ShipChanged += ModelShipChangedEventHandler;
            _model.ShotMoved += ModelShotMovedEventHandler;
            _model.StarChanged += ModelStarChangedEventHandler;
            _model.NextWaveGenerated += ModelNextWaveGeneratedEventHandler;
            _model.GameLost += ModelGameLostHandler;

            _timer.Interval = TimeSpan.FromMilliseconds(85);
            _timer.Tick += TimerTickEventHandler;

            EndGame();
        }

        public INotifyCollectionChanged Sprites => _sprites;

        public bool GameOver => _model.GameOver;

        public INotifyCollectionChanged Lives => _lives;

        public int LivesValue => _lives.Count;

        public bool Paused { get; set; }

        public bool IsHighestScore => Score > _model.HistoryData.HighestScore;

        public static double Scale { get; private set; }

        public int Score { get; private set; }

        public Size PlayAreaSize
        {
            set
            {
                Scale = value.Width / 405;
                _model.UpdateAllShipsAndStars();
                RecreateScanLines();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TimerTickEventHandler(object sender, EventArgs e)
        {
            if (_lastPaused != Paused)
            {
                OnPropertyChanged(nameof(Paused));
                _lastPaused = Paused;
            }

            if (!Paused)
            {
                if (_leftAction.HasValue && !_rightAction.HasValue)
                    _model.MovePlayer(Direction.Left);
                else if (!_leftAction.HasValue && _rightAction.HasValue)
                    _model.MovePlayer(Direction.Right);
                else if (_leftAction.HasValue && _rightAction.HasValue)
                    _model.MovePlayer(_leftAction > _rightAction ? Direction.Left : Direction.Right);
            }

            _model.Update(Paused);

            if (Score != _model.Score)
            {
                Score = _model.Score;
                OnPropertyChanged(nameof(Score));
            }

            if (_model.Lives > 0)
            {
                while (_lives.Count > _model.Lives)
                    _lives.RemoveAt(0);
                while (_lives.Count < _model.Lives)
                    _lives.Add(new object());
                OnPropertyChanged(nameof(LivesValue));
            }

            foreach (var control in _shotInvaders.Keys.ToList())
            {
                var elapsed = _shotInvaders[control];
                if (DateTime.Now - elapsed > TimeSpan.FromSeconds(0.5))
                {
                    _sprites.Remove(control);
                    _shotInvaders.Remove(control);
                }
            }

            if (_model.GameOver)
            {
                //Game over!
                OnPropertyChanged(nameof(GameOver));
                _timer.Stop();
            }
        }

        private void ModelStarChangedEventHandler(object sender, StarChangedEventArgs e)
        {
            if (e.Disappeared && _stars.ContainsKey(e.Point))
            {
                _sprites.Remove(_stars[e.Point]);
                _stars.Remove(e.Point);
            }
            else
            {
                if (!_stars.ContainsKey(e.Point))
                {
                    var starControl = InvadersHelper.StarControlFactory(e.Point, Scale);
                    _stars.Add(e.Point, starControl);
                    _sprites.Add(starControl);
                }
                else
                {
                    var starControl = _stars[e.Point];
                    InvadersHelper.MoveElementOnCanvas(starControl,
                        e.Point.X * Scale,
                        e.Point.Y * Scale);
                }
            }
        }

        private void ModelShotMovedEventHandler(object sender, ShotMovedEventArgs e)
        {
            if (!e.Disappeared)
            {
                if (!_shots.Keys.Contains(e.Shot))
                {
                    var shotControl = InvadersHelper.ShotControlFactory(e.Shot, Scale);
                    _shots.Add(e.Shot, shotControl);
                    _sprites.Add(shotControl);
                }
                else
                {
                    var shotControl = _shots[e.Shot];
                    InvadersHelper.MoveElementOnCanvas(shotControl,
                        e.Shot.Location.X * Scale,
                        e.Shot.Location.Y * Scale);
                }
            }
            else
            {
                if (_shots.ContainsKey(e.Shot))
                {
                    _sprites.Remove(_shots[e.Shot]);
                    _shots.Remove(e.Shot);
                }
            }
        }

        private void ModelShipChangedEventHandler(object sender, ShipChangedEventArgs e)
        {
            if (!e.Killed)
                switch (e.ShipUpdated)
                {
                    case Invader _:
                        var invader = e.ShipUpdated as Invader;
                        if (!_invaders.ContainsKey(invader))
                        {
                            var fe = InvadersHelper.InvaderControlFactory(invader, Scale);
                            _invaders.Add(invader, fe);
                            _sprites.Add(fe);
                        }
                        else
                        {
                            var fe = _invaders[invader];
                            InvadersHelper.MoveElementOnCanvas(fe,
                                invader.Location.X * Scale,
                                invader.Location.Y * Scale);
                            InvadersHelper.ResizeElement(fe,
                                invader.Size.Width * Scale,
                                invader.Size.Height * Scale);
                        }

                        break;
                    case Player _:
                        if (_playerFlashing)
                        {
                            var playerControl = _playerControl as AnimatedImage;
                            playerControl?.StopFlashing();
                            _playerFlashing = false;
                        }

                        var player = e.ShipUpdated as Player;
                        if (_playerControl == null)
                        {
                            _playerControl = InvadersHelper.PlayerControlFactory(player, Scale);
                            _sprites.Add(_playerControl);
                        }
                        else
                        {
                            InvadersHelper.MoveElementOnCanvas(_playerControl,
                                player.Location.X * Scale,
                                player.Location.Y * Scale);
                            InvadersHelper.ResizeElement(_playerControl,
                                player.Size.Width * Scale,
                                player.Size.Height * Scale);
                        }

                        break;
                }
            else
                switch (e.ShipUpdated)
                {
                    case Invader _:
                        var invader = e.ShipUpdated as Invader;
                        if (!_invaders.ContainsKey(invader)) return;
                        if (_invaders[invader] is AnimatedImage invaderControl)
                        {
                            invaderControl.InvaderShot();
                            _shotInvaders[invaderControl] = DateTime.Now;
                            _invaders.Remove(invader);
                        }

                        break;
                    case Player _:
                        var playerControl = (AnimatedImage) _playerControl;
                        playerControl.StartFlashing();
                        _playerFlashing = true;
                        OnPlayerShot();
                        break;
                }
        }

        private void ModelNextWaveGeneratedEventHandler(object sender, EventArgs e)
        {
            OnNextWaveGenerated();
        }

        private void ModelGameLostHandler(object sender, EventArgs e)
        {
            OnGameLost();
        }

        private void RecreateScanLines()
        {
            foreach (var scanLine in _scanLines)
                if (_sprites.Contains(scanLine))
                    _sprites.Remove(scanLine);
            _scanLines.Clear();
            for (var y = 0; y < 300; y += 2)
            {
                var scanLine = InvadersHelper.ScanLineFactory(y, 400, Scale);
                _scanLines.Add(scanLine);
                _sprites.Add(scanLine);
            }
        }

        private void EndGame()
        {
            _model.EndGame();
        }

        public void StartGame()
        {
            Paused = false;
            foreach (var invader in _invaders.Values) _sprites.Remove(invader);
            foreach (var shot in _shots.Values) _sprites.Remove(shot);
            _model.StartGame();
            OnPropertyChanged(nameof(GameOver));
            _timer.Start();
        }

        internal void KeyDown(Key key)
        {
            switch (key)
            {
                case Key.D1:
                    _model.CurrentShotType = ShotType.BasicShot;
                    break;
                case Key.D2:
                    _model.CurrentShotType = ShotType.LazerShot;
                    break;
                case Key.Space:
                    _model.FireShot();
                    break;
                case Key.A:
                    _leftAction = DateTime.Now;
                    break;
                case Key.D:
                    _rightAction = DateTime.Now;
                    break;
                case Key.Escape:
                    Paused = !Paused;
                    break;
            }
        }

        internal void KeyUp(Key key)
        {
            switch (key)
            {
                case Key.A:
                    _leftAction = null;
                    break;
                case Key.D:
                    _rightAction = null;
                    break;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler NextWaveGenerated;

        private void OnNextWaveGenerated()
        {
            var nextWave = NextWaveGenerated;
            nextWave?.Invoke(this, new EventArgs());
        }

        public event EventHandler GameLost;

        private void OnGameLost()
        {
            var gameLost = GameLost;
            gameLost?.Invoke(this, new EventArgs());
            OnPropertyChanged(nameof(IsHighestScore));
        }

        public event EventHandler PlayerShot;

        private void OnPlayerShot()
        {
            var playerShot = PlayerShot;
            playerShot?.Invoke(this, new EventArgs());
        }
    }
}