using MVVMHelpers;
using NoteModel;
using Prism.Commands;
using SharedLibraries.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XTestMan.Views.Music
{
    public class SheetViewModel : ViewModelBase, INoteListener, IMidiPublisher,INavigationSource
    {
        private Sheet _model;

        public ObservableCollection<NoteSection> Bars { get; set; }

        public SheetViewModel()
        {
            RandomizeCommand = new DelegateCommand(OnRandomize);
            _model = new Sheet(Clef.Treble);
        }
 
        private void OnRandomize()
        {
            //TrebleNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateRandomSectionsFromClef(Clef.Treble));
            //BassNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateRandomSectionsFromClef(Clef.Bass));
            Name = "Sheet";
            TrebleNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateGroups(Clef.Treble, 8, 3, false).Select(x => new PlayingNoteViewModel(x)));
            BassNotes = new ObservableCollection<NoteSection>(NoteReader.RandomNoteReader.CreateGroups(Clef.Bass, 8, 3, true).Select(x => new PlayingNoteViewModel(x)));
            OnPropertyChanged("TrebleNotes");
            OnPropertyChanged("BassNotes");
        } 

        public ObservableCollection<Note> Notes { get; set; }

        private string _textNotes;

        public string TextNotes
        {
            get { return _textNotes; }
            set { _textNotes = value; OnPropertyChanged();
                //SetNotes(value);
            }
        }
 
        //private void SetNotes(string value)
        //{
        //    Bars.Clear();

        //    var bars = value.Split(',');
        //    foreach(var section in bars)
        //    {
        //        if (string.IsNullOrEmpty(section))
        //            break;

        //        var res = new List<Note>();

        //        foreach(var c in section.ToCharArray())
        //        {
        //            var notes = Sheet.GetNotesInActiveClef(_model.ActiveClef);
        //            var val = _model.GetNoteValue(_model.ActiveClef,new Note(c.ToString()));
        //            var note = new Note(c.ToString()) { Value = val };
        //            res.Add(note);
        //        }

        //        Bars.Add(NoteSection.CreateSectionFromNotes(res,_model));
        //    }

        //    PlayingBars = new ObservableCollection<NoteSection>( Bars.Select(x => new PlayingNoteViewModel(x)) );



        //    OnPropertyChanged("Bars");
        //    OnPropertyChanged("PlayingBars");
        //}

        public void OnNotePressed(int note)
        {
            OnNotesPressed(new List<int>() { note });
        } 

        private NoteSection FirstUnplayedInSequence(List<NoteSection> sections, out int foundAt)
        {
            var hasUnplayedNotes = sections.Any(x => x.AllNotes.Count > 0 && !(x as PlayingNoteViewModel).IsPlayed);

            //TODO deal with this 
            if(!hasUnplayedNotes)
            {
                foundAt = int.MaxValue;
                return new NoteSection();
            }

            var res =  sections.Select((value,index) => new {section=value,index=index}).First(x => x.section.AllNotes.Count > 0 && !(x.section as PlayingNoteViewModel).IsPlayed); 
            foundAt = res.index;
            return res.section;
        }

        public NoteSection CurrentNoteSection()
        {
            var ts = FirstUnplayedInSequence(TrebleNotes.ToList(),out var treb);
            var bs = FirstUnplayedInSequence(BassNotes.ToList(), out var bass);

            if(treb == bass)
            {
                var notes = ts.AllNotes.Union(bs.AllNotes).ToList();
                //TODO check this out, what should this do with clef and sheet?
                return NoteSection.CreateSectionFromNotes(notes.ToList(),Clef.Treble,_model);
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


            var allPlayed = firstUnplayed.AllNotes.Select(x => x.Id.ToUpper()).All(x => playedNoteNames.Contains(x));
            if (allPlayed)
            {
                MarkLastAsPlayed();
            }

            var anyUnplayed = CurrentNotes().Select(x => (PlayingNoteViewModel)x).Any(x => !x.IsPlayed);
            if(!anyUnplayed)
            {
                OnRandomize();
            } 
        }

        private void MarkLastAsPlayed()
        {
            var ts = FirstUnplayedInSequence(TrebleNotes.ToList(), out var ti);
            var bs = FirstUnplayedInSequence(BassNotes.ToList(), out var bi);

            if(ti == bi)
            {
                (ts as PlayingNoteViewModel).IsPlayed = true;
                (bs as PlayingNoteViewModel).IsPlayed = true; 
            }
            else
            {
                var section = ti < bi ? ts : bs;
                (section as PlayingNoteViewModel).IsPlayed = true;
            }
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
                //TODO renable this for single  clef mode
                //_model.SwitchClef(value);
                OnPropertyChanged();
                //SetNotes(TextNotes);
            }

        }

        public ObservableCollection<String> AvailableDevices { get; set; }

        public ObservableCollection<NoteSection> PlayingBars { get; set; }

        public ObservableCollection<NoteSection> TrebleNotes { get; set; }
        public ObservableCollection<NoteSection> BassNotes { get; set; }

        private String _selectedDevice;

        public event EventHandler MidiDeviceChanged;
        public event EventHandler<MidiKeyEventArgs> OnKeyPressed;
        public event EventHandler<MidiKeyEventArgs> OnKeyReleased;

        public String SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; MidiDeviceChanged(this,new MidiListenerEventArgs() { SelectedDevice = _selectedDevice }); }
        }

        private bool _hasSelectedDevice;
        private static Random rand = new Random();

        public bool HasSelectedDevice 
        {
            get { return _hasSelectedDevice; }
            set { _hasSelectedDevice = value; OnPropertyChanged(); }
        }

        public string Name { get; set; }
        public ICommand OnSelected { get; set; }
    }
}
