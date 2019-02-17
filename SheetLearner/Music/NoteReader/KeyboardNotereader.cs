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
    class KeyboardNoteReader : ViewModelBase,INoteReader 
    {
        private INoteListener _noteListener;
        private IMidiListener _listener;
        private List<int> _pressedKeys;
        private Dictionary<char, int> _keyDict;

        public KeyboardNoteReader(INoteListener nl, IMidiListener list)
        {
            _noteListener = nl;
            _listener = list;
            KeyPressedCommand = new DelegateCommand<object>(OnKeyPressed);
            KeyUpCommand = new DelegateCommand<object>(OnKeyUp);
            _pressedKeys = new List<int>();

            _keyDict = new Dictionary<char, int>()
            {
                {'c',0}, //C
                {'d',2}, //D
                {'e',4}, //E

                {'f',5}, //F
                {'g',7}, //G
                {'a',9},//A

                {'b',11}, //B
                {'k',8}, //G
                {'l',8}, //G

                {'m',8}, //G
                {'n',8}, //G 
            };
        }

        private int MapToKey(object ob)
        {
            var strTest = (Key)ob;
            var strT = strTest.ToString().ToLower();
            var character = (char)strT.First();
            if(_keyDict.ContainsKey(character))
            {
                var da = _keyDict[character];
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

        public void SelectDefaultDevice()
        {
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
