using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTestMan.Views.Music.Interfaces
{
    public interface INoteReader
    {
        List<NoteSection> GetNoteSections();
    }
}
