﻿using System.Windows.Input;

namespace SharedLibraries.Interfaces
{
	public interface INavigationSource
	{
		string Name { get; set; }
		ICommand OnSelected { get; set; }
	}
}
