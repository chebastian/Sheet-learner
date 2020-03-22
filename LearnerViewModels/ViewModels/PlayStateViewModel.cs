using Sefe.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Music.ViewModels
{
    public class PlayStateViewModel : ViewModelBase
    {
        public interface IGameCompleteListener
        {
            void OnCompleted();
        }

        private double elapsedTime;

        public PlayStateViewModel(IGameCompleteListener listener)
        {
            _listener = listener;
            RunningTime = TimeSpan.FromSeconds(20);
            ElapsedTime = 0.0;
            StartCommand = new RelayCommand(Start);
            ResetCommand = new RelayCommand(Start);
        }

        public async void Start()
        {
            Score = 0;
            IsRunning = true;
            await Task.Delay(RunningTime);
            IsRunning = false;
            Completed = true;
            _running = false;
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

        internal void NotePressed(List<string> allPlayed)
        {
            if (_running)
            {
                Score += allPlayed.Count;
            }
        }
    }
}