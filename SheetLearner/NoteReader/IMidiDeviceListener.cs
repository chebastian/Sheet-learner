using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoteModel
{
	public interface IMidiDeviceListener
    {
        void OnDeviceSelected(INotePublisher name);
    }
}
