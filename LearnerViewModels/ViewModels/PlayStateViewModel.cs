using Sefe.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Music.ViewModels
{
    internal class PlayStateViewModel : ViewModelBase
    {
        public interface IGameCompleteListener
        {
            void OnCompleted();
        }

        private double elapsedTime;

        public PlayStateViewModel(IGameCompleteListener listener)
        {
            _listener = listener;
            RunningTime = TimeSpan.FromMinutes(2.0);
            ElapsedTime = 0.0;
        }

        public async void Start()
        {
            Score = 0;
            _running = true;
            await Task.Delay(RunningTime);
            _listener.OnCompleted();
            _running = false;
        }

        private IGameCompleteListener _listener;
        private bool _running;

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

        public int Score { get; private set; }

        internal void NotePressed(List<string> allPlayed)
        { 
            if(_running)
            {
                Score += allPlayed.Count;
            }
        }
    }
}