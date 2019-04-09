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
        private Sheet _model;

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
            _model = new Sheet(Clef.Treble);
            ClefViewModel = new ClefViewModel(Clef.Treble);
            BassClefViewModel = new ClefViewModel(Clef.Bass);
        }

        private void OnRandomize()
        {
            //var tn = NotesFactory.TrebleNote.Select(x => new PlayingNoteViewModel(new NoteSection(new List<NoteViewModel>() { new NoteViewModel(x) })));
            //TrebleNotes = new ObservableCollection<NoteSection>(tn);
            //var bn = NotesFactory.BassNote.Select(x => new PlayingNoteViewModel(new NoteSection(new List<NoteViewModel>() { new NoteViewModel(x) })));
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

        public ObservableCollection<Note> Notes { get; set; }

        private string _textNotes;

        public string TextNotes
        {
            get { return _textNotes; }
            set
            {
                _textNotes = value; OnPropertyChanged();
                //SetNotes(value);
            }
        }


        public void OnNotePressed(int note)
        {
            OnNotesPressed(new List<int>() { note });
        }

        private NoteSection FirstUnplayedInSequence(List<NoteSection> sections, out int foundAt)
        {
            //var hasUnplayedNotes = sections.Any(x => x.Notes.Count > 0 && !(x as NoteViewModel).IsPlayed);

            ////TODO deal with this 
            //if (!hasUnplayedNotes)
            //{
            //    foundAt = int.MaxValue;
            //    return new NoteSection();
            //}

            var res = sections.Select((value, index) => new { section = value, index = index }).First(x => x.section.Notes.Count > 0 && !(x.section.IsAllPlayed()));
            foundAt = res.index;
            return res.section;
        }

        public NoteSection CurrentNoteSection()
        {
            //var ts = FirstUnplayedInSequence(TrebleNotes.ToList(), out var treb);
            //var bs = FirstUnplayedInSequence(BassNotes.ToList(), out var bass);
            var ts = FirstUnplayedInSequence(ClefViewModel.Groups, out var treb);
            var bs = FirstUnplayedInSequence(BassClefViewModel.Groups, out var bass);

            if (treb == bass)
            {
                var notes = ts.Notes.Union(bs.Notes).ToList();
                //TODO check this out, what should this do with clef and sheet?
                //return NoteSection.CreateSectionFromNotes(notes.ToList(), Clef.Treble, _model);
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
            var scale = "C,C#,D,D#,E,F,F#,G,G#,A,A#,B";
            var scaleArr = scale.Split(',');

            var firstUnplayed = CurrentNoteSection();

            var playedNoteNames = playedNotes.Select(x => scaleArr[x]).ToList();
            var pnotes = playedNotes.Select(x => new Note(scaleArr[x])).ToList();


            //var allPlayed = firstUnplayed.Notes.Select(x => x.Note.Id.ToUpper().Take(1)).All(x => playedNoteNames.Contains(x));
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
            //var ts = FirstUnplayedInSequence(TrebleNotes.ToList(), out var ti);
            //var bs = FirstUnplayedInSequence(BassNotes.ToList(), out var bi);
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

        public Clef ActiveClef
        {
            get
            {
                return _model.ActiveClef;
            }
            set
            {
                OnPropertyChanged();
            }

        }

        public ObservableCollection<String> AvailableDevices { get; set; }
        public ObservableCollection<NoteSection> PlayingBars { get; set; }
        public ObservableCollection<NoteSection> TrebleNotes { get; set; }
        public ObservableCollection<NoteSection> BassNotes { get; set; }

        private String _selectedDevice;
        public String SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        private bool _hasSelectedDevice;
        private static Random rand = new Random();
        private string _name;

        public bool HasSelectedDevice
        {
            get { return _hasSelectedDevice; }
            set { _hasSelectedDevice = value; OnPropertyChanged(); }
        }

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public ICommand OnSelected { get; set; }
    }
}
