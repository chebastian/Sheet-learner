using MVVMHelpers;
using NoteModel;
using Prism.Commands;
using SharedLibraries.Interfaces;
using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XTestMan.Views.Music
{
    public class SheetViewModel : ViewModelBase, INoteListener, INavigationSource
    { 
        public ObservableCollection<NoteSection> Bars { get; set; }


        private ClefViewModel _bassClefViewModel;
        public ClefViewModel BassClefViewModel
        {
            get => _bassClefViewModel;
            set
            {
                _bassClefViewModel = value;
                OnPropertyChanged();
            }
        }

        private ClefViewModel _clefView;
        public ClefViewModel ClefViewModel
        {
            get => _clefView;
            set
            {
                _clefView = value;
                OnPropertyChanged();
            }
        }


        public SheetViewModel()
        {
            RandomizeCommand = new DelegateCommand(OnRandomize);
            Name = "Sheet";
            ClefViewModel = new ClefViewModel(Clef.Treble);
            BassClefViewModel = new ClefViewModel(Clef.Bass);
        }

        private void OnRandomize()
        {
            //var tn = NotesFactory.TrebleNotes.Select(x => new PlayingNoteViewModel(new NoteSection(new List<NoteViewModel>() { new NoteViewModel(x) })));
            //TrebleNotes = new ObservableCollection<NoteSection>(tn);
            //var bn = NotesFactory.BassNotes.Select(x => new PlayingNoteViewModel(new NoteSection(new List<NoteViewModel>() { new NoteViewModel(x) })));
            //BassNotes = new ObservableCollection<NoteSection>(bn);

            TrebleNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateGroups(Clef.Treble, 8, 3, false).Select(x => new PlayingNoteViewModel(x)));
            BassNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateGroups(Clef.Bass, 8, 3, true).Select(x => new PlayingNoteViewModel(x)));

            foreach (var section in TrebleNotes)
            {
                ClefViewModel.AddNoteGroup(section.AllNotes);
            }

            foreach(var section in BassNotes)
            {
                BassClefViewModel.AddNoteGroup(section.AllNotes);
            }

            OnPropertyChanged("TrebleNotes");
            OnPropertyChanged("BassNotes");
        }
 
        public void OnNotePressed(int note)
        {
            OnNotesPressed(new List<int>() { note });
        }

        private NoteSection FirstUnplayedInSequence(List<NoteSection> sections, out int foundAt)
        { 
            var res = sections.Select((value, index) => new { section = value, index = index }).First(x => x.section.Notes.Count > 0 && !(x.section.IsAllPlayed()));
            foundAt = res.index;
            return res.section;
        }

        public NoteSection CurrentNoteSection()
        {
            var ts = FirstUnplayedInSequence(ClefViewModel.Groups, out var treb);
            var bs = FirstUnplayedInSequence(BassClefViewModel.Groups, out var bass);

            if (treb == bass)
            {
                var notes = ts.Notes.Union(bs.Notes).ToList();
                return new NoteSection(notes);
            }

            return treb < bass ? ts : bs;
        }

        public List<NoteSection> CurrentNotes()
        {
            return TrebleNotes.ToList();
        }

        public void OnNotesPressed(List<int> playedNotes)
        {

            var firstUnplayed = CurrentNoteSection();

            var scaleArr = "C,C#,D,D#,E,F,F#,G,G#,A,A#,B".Split(',');
            var playedNoteNames = playedNotes.Select(x => scaleArr[x]).ToList();

            var allPlayed = firstUnplayed.Notes.Select(x => x.Note.Id.ToUpper().Substring(0,1)).ToList();
            var isAllPlayed = allPlayed.All(x => playedNoteNames.Contains(x));
            if (isAllPlayed)
            {
                MarkLastAsPlayed();
            }

            var anyUnplayed = CurrentNotes().Select(x => (PlayingNoteViewModel)x).Any(x => !x.IsPlayed);
            if (!anyUnplayed)
            {
                OnRandomize();
            }
        }

        private void MarkLastAsPlayed()
        {
            var ts = FirstUnplayedInSequence(ClefViewModel.Groups,out var ti);
            var bs = FirstUnplayedInSequence(BassClefViewModel.Groups,out var bi);

            if (ti == bi)
            {
                ts.SetAllPlayed();
                bs.SetAllPlayed();
            }
            else
            {
                var section = ti < bi ? ts : bs;
                (section).SetAllPlayed();
            }
        }

        public void OnNoteReleased(int note)
        {
        }

        private ICommand _command;

        public ICommand RandomizeCommand
        {
            get { return _command; }
            set { _command = value; OnPropertyChanged(); }
        }
        public ObservableCollection<NoteSection> TrebleNotes { get; set; }
        public ObservableCollection<NoteSection> BassNotes { get; set; }


        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public ICommand OnSelected { get; set; }
    }
}
