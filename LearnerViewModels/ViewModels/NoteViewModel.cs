using Sefe.Utils.MVVM;
using SheetLearner.Music;

namespace Music.ViewModels
{
	public class NoteViewModel : ViewModelBase
	{

		public NoteViewModel()
		{
			StemEnd = 0;
			StemX = 0;
			StemY = 0;
		}

		public int StemEnd { get; set; }

		public NoteViewModel(Note note)
		{
			Note = note;
			Played = false;
		}

		private bool _played;

		public Note Note { get; }

		public bool Played
		{
			get => _played;
			set
			{
				_played = value;
				OnPropertyChanged();
			}
		}

		private int _x;
		public int X
		{
			get => _x;
			set
			{
				_x = value;
				OnPropertyChanged();
			}
		}


		private int _y;
		public int Y
		{
			get => _y;
			set
			{
				_y = value;
				OnPropertyChanged();
			}
		}

		public int StemX { get; internal set; }
		public int StemY { get; internal set; }
	}
}
