using MVVMHelpers;
using NoteModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XTestMan.Views.Music.NoteReader
{
    public class WpfKeyReader : IMidiPublisher
    {
        public WpfKeyReader()
        { 
        }

        private INoteListener _listener;

        public void Register(INoteListener listener)
        {
            _listener = listener;
        }

        public void Unregister(INoteListener listener)
        {
        }
    }

    class KeyboardNoteReader : ViewModelBase,IMidiRepository 
    {
        private INoteListener _noteListener;
        private IMidiPublisher _listener;
        private List<int> _pressedKeys;
        private static Dictionary<char, int> KeyDictionary => new Dictionary<char, int>()
            {
                {'c',0}, 
                {'d',2}, 
                {'e',4}, 

                {'f',5}, 
                {'g',7}, 
                {'a',9}, 

                {'b',11},
                {'k',8}, 
                {'l',8}, 

                {'m',8}, 
                {'n',8}, 
            };


        public KeyboardNoteReader(INoteListener nl, IMidiPublisher list)
        {
            _noteListener = nl;
            _listener = list;
            KeyPressedCommand = new DelegateCommand<object>(OnKeyPressed);
            KeyUpCommand = new DelegateCommand<object>(OnKeyUp);
            _pressedKeys = new List<int>();
        }

        public static int MapToKey(object ob)
        {
            var strTest = (Key)ob;
            var strT = strTest.ToString().ToLower();
            var character = (char)strT.First();
            if(KeyDictionary.ContainsKey(character))
            {
                var da = KeyDictionary[character];
                return da;
            }
            return -1;
        }

        private void OnKeyUp(object obj)
        {
            _pressedKeys.Remove(MapToKey(obj));
            Debug.WriteLine($"released {obj}");
        }

        private void OnKeyPressed(object key)
        {
             var ikey = MapToKey(key);
            if (ikey == -1)
                return;

            _pressedKeys.Add(ikey);

            //_noteListener.OnNotePressed(ikey);
            _noteListener.OnNotesPressed(_pressedKeys);
        } 
 
        public IMidiPublisher GetCurrentPublisher()
        {
            throw new NotImplementedException();
        }

        public IMidiPublisher GetPublisherWithName(string name)
        {
            throw new NotImplementedException();
        }

        private ICommand _onKeyPressed;

        public ICommand KeyPressedCommand
        {
            get { return _onKeyPressed; }
            set { _onKeyPressed = value; OnPropertyChanged(); }
        }

        private ICommand _keyUpCommand;

        public ICommand KeyUpCommand
        {
            get { return _keyUpCommand; }
            set { _keyUpCommand = value; OnPropertyChanged();
            }
        }

        public List<string> AvailableDevices { get; set; }
    }
}
