using MVVMHelpers;
using SharedLibraries.Interfaces;
using System.Windows.Input;

namespace SharedLibraries
{
	public class NavigationSource : ViewModelBase, INavigationSource
	{

		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}


		private ICommand command;
		public ICommand OnSelected
		{
			get => command;
			set
			{
				command = value;
				OnPropertyChanged();
			}
		}
	}

	public class NavigationA : NavigationSource { }
	public class NavigationB : NavigationSource { }
}