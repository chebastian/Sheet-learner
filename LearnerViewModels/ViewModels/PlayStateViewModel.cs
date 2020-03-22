using Sefe.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Music.ViewModels
{
    public class PlayStateViewModel : ViewModelBase
    {
        public interface IGameCompleteListener
        {
            void OnStart();
            void OnCompleted();
        }

        private double elapsedTime;

        public PlayStateViewModel(IGameCompleteListener listener)
        {
            _listener = listener;
            ShowPlayButton = true;
            RunningTime = TimeSpan.FromSeconds(20);
            ElapsedTime = 0.0;
            StartCommand = new RelayCommand(Start);
            ResetCommand = new RelayCommand(Start);
        }

        public async void Start()
        {
            Score = 0;
            IsRunning = true;
            ShowPlayButton = false;
            using (var dispatch = new Timer(OnTimerCallback, null, 0, 20))
            {
                StartTime = DateTime.Now;
                ElapsedTime = 0;
                _listener.OnStart();
                await Task.Delay(RunningTime);
                _listener.OnCompleted();
                IsRunning = false;
                Completed = true;
                _running = false;
                ShowPlayButton = true;
            }

        }

        private void OnTimerCallback(object state)
        {
            var d = DateTime.Now - StartTime;
            ElapsedTime = d.TotalSeconds;
        }

        public bool IsRunning
        {
            get => _running;
            set
            {
                _running = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPlayButton
        {
            get => showPlayButton;
            set
            {
                showPlayButton = value;
                OnPropertyChanged();
            }
        }

        private bool _completed;

        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; OnPropertyChanged(); }
        }

        public ICommand StartCommand
        {
            get; set;
        }

        public ICommand ResetCommand
        {
            get; set;
        }

        private IGameCompleteListener _listener;
        private bool _running;
        private int score;
        private bool showPlayButton;

        public TimeSpan RunningTime { get; }
        public double ElapsedTime
        {
            set
            {
                elapsedTime = value;
                OnPropertyChanged();
            }
            get
            {
                return elapsedTime;
            }
        }

        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartTime { get; private set; }

        internal void NotePressed(List<string> allPlayed)
        {
            if (_running)
            {
                Score += 1;
            }
        }
    }
}