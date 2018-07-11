using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Invaders.Wpf.Annotations;
using Invaders.Wpf.Model;
using Invaders.Wpf.View;

namespace Invaders.Wpf.ViewModel
{
    public class InvadersViewModel : INotifyPropertyChanged
    {
        private readonly  ObservableCollection<FrameworkElement> _sprites = 
            new ObservableCollection<FrameworkElement>();

        public INotifyCollectionChanged Sprites => _sprites;

        public bool GameOver => _model.GameOver;
        
        private readonly ObservableCollection<object> _lives = 
            new ObservableCollection<object>();

        public INotifyCollectionChanged Lives => _lives;

        public int LivesValue => _lives.Count;
        
        public bool Paused { get; set; }
        
        private bool _lastPaused = true;
        
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

        private readonly InvadersModel _model = new InvadersModel();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private FrameworkElement _playerControl = null;
        private bool _playerFlashing = false;
        private readonly Dictionary<Invader, FrameworkElement> _invaders = 
            new Dictionary<Invader, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders =
            new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<Shot, FrameworkElement> _shots =
            new Dictionary<Shot, FrameworkElement>();
        private readonly Dictionary<Point, FrameworkElement> _stars =
            new Dictionary<Point, FrameworkElement>();
        private readonly List<FrameworkElement> _scanLines =
            new List<FrameworkElement>();

        private DateTime? _leftAction = null;
        private DateTime? _rightAction = null;

        public InvadersViewModel()
        {
            Scale = 1;
            
            _model.ShipChanged += ModelShipChangedEventHandler;
            _model.ShotMoved += ModelShotMovedEventHandler;
            _model.StarChanged += ModelStarChangedEventHandler;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerTickEventHandler;

            EndGame();
        }

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
                {
                    _model.MovePlayer(Direction.Left);
                } else if (!_leftAction.HasValue && _rightAction.HasValue)
                {
                    _model.MovePlayer(Direction.Right);
                } else if (_leftAction.HasValue && _rightAction.HasValue)
                {
                    _model.MovePlayer(_leftAction > _rightAction ? Direction.Left : Direction.Right);
                }
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
            }

            foreach (var control in _shotInvaders.Keys.ToList())
            {
                DateTime elapsed = _shotInvaders[control];
                if (DateTime.Now - elapsed > TimeSpan.FromSeconds(0.5))
                {
                    _sprites.Remove(control);
                    _shotInvaders.Remove(control);
                }
            }

            if (_model.GameOver)
            {
                OnPropertyChanged(nameof(GameOver));
                _timer.Stop();
            }
        }

        private void ModelStarChangedEventHandler(object sender, StarChangedEventArgs e)
        {
            throw new NotImplementedException(nameof(ModelStarChangedEventHandler));
        }

        private void ModelShotMovedEventHandler(object sender, ShotMovedEventArgs e)
        {
            throw new NotImplementedException(nameof(ModelShotMovedEventHandler));
        }

        private void ModelShipChangedEventHandler(object sender, ShipChangedEventArgs e)
        {
            if (!e.Killed)
            {
                if (e.ShipUpdated is Invader)
                {
                    Invader invader = e.ShipUpdated as Invader;
                    if (!_invaders.ContainsKey(invader))
                    {
                        FrameworkElement fe = InvadersHelper.InvaderControlFactory(invader, Scale);
                        _invaders.Add(invader, fe);
                        _sprites.Add(fe);
                    }
                    else
                    {
                        FrameworkElement fe = _invaders[invader];
                        InvadersHelper.MoveElementOnCanvas(fe, 
                            invader.Location.X * Scale, 
                            invader.Location.Y * Scale);
                        InvadersHelper.ResizeElement(fe, 
                            invader.Size.Width * Scale, 
                            invader.Size.Height * Scale);
                    }
                } else if (e.ShipUpdated is Player)
                {
                    if (_playerFlashing)
                    {
                        AnimatedImage playerControl = _playerControl as AnimatedImage;
                        playerControl?.StopFlashing();
                        _playerFlashing = false;
                    }
                    
                    Player player = e.ShipUpdated as Player;
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
                }
            }
            else
            {
                if (e.ShipUpdated is Invader)
                {
                    Invader invader = e.ShipUpdated as Invader;
                    if (!_invaders.ContainsKey(invader)) return;
                    AnimatedImage invaderControl = _invaders[invader] as AnimatedImage;
                    if (invaderControl != null)
                    {
                        invaderControl.InvaderShot();
                        _shotInvaders[invaderControl] = DateTime.Now;
                        _invaders.Remove(invader);
                    }
                } else if (e.ShipUpdated is Player)
                {
                    AnimatedImage playerControl = (AnimatedImage) _playerControl;
                    playerControl.StartFlashing();
                    _playerFlashing = true;
                }
            }
        }
        
        private void RecreateScanLines()
        {
            foreach (var scanLine in _scanLines)
            {
                if (_sprites.Contains(scanLine))
                {
                    _sprites.Remove(scanLine);
                }
            }
            _scanLines.Clear();
            for (int y = 0; y < 300; y+=2)
            {
                FrameworkElement scanLine = InvadersHelper.ScanLineFactory(y, 400, Scale);
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
            if (key == Key.Space)
            {
                _model.FireShot();
            }

            if (key == Key.Left)
            {
                _leftAction = DateTime.Now;
            }

            if (key == Key.Right)
            {
                _rightAction = DateTime.Now;
            }
        }

        internal void KeyUp(Key key)
        {
            if (key == Key.Left)
            {
                _leftAction = null;
            }

            if (key == Key.Right)
            {
                _rightAction = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}