using System.Windows.Input;
using SharedLibraries.Interfaces;

namespace SharedLibraries
{
    internal class Settings : INavigationSource
    {
        public string Name { get; set; }
        public ICommand OnSelected { get; set; }

        public Settings()
        {
            Name = "Settings";
        }
    }
}