using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharedLibraries.Interfaces
{
    public interface INavigationSource
    {
        string Name { get; set; }
        ICommand OnSelected { get; set; } 
    }
}
