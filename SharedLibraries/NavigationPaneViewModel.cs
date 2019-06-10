﻿using MVVMHelpers;
using SharedLibraries.Interfaces;
using System.Collections.ObjectModel;

namespace SharedLibraries
{
	public class NavigationPaneViewModel : ViewModelBase
	{
		public ObservableCollection<INavigationSource> NavigationSource { get; set; }


		private INavigationSource _selectedSource;
		public INavigationSource SelectedSource
		{
			get => _selectedSource;
			set
			{
				_selectedSource = value;
				_selectedSource.OnSelected?.Execute(null);
				OnPropertyChanged();
			}
		}

		public NavigationPaneViewModel()
		{
			NavigationSource = new ObservableCollection<INavigationSource>();


			NavigationSource.Add(new NavigationA() { Name = "100 Notes Challange" });
			NavigationSource.Add(new NavigationB() { Name = "5 Minute Countdown" });
			NavigationSource.Add(new Settings());
		}
	}
}
