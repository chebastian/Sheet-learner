using MVVMHelpers;
using NoteModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
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

        public KeyboardNoteReader(INoteListener nl, IMidiListener list)
        {
            _noteListener = nl;
            _listener = list;
            KeyPressedCommand = new DelegateCommand<object>(OnKeyPressed);
            KeyUpCommand = new DelegateCommand<object>(OnKeyUp);
        }

        private void OnKeyUp(object obj)
        {
        }

        private void OnKeyPressed(object key)
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

    }
}
