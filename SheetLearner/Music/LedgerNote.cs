using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTestMan.Views.Music
{
    public class LedgerNote : Note
    {
        public LedgerNote(Note note, bool isLine)
            :base(note)
        {
            ShowLine = note.Show && isLine;
        }

        private bool _showLine;

        public bool ShowLine
        {
            get { return _showLine; }
            set { _showLine = value; OnPropertyChanged(); }
        } 
    }
}
