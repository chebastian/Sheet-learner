﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XTestMan.Utils.Converter
{
	public class BoolToVisibility : IValueConverter
	{
		public Visibility FalseValue;
		public Visibility TrueValue;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var b = (bool)value;

			return b ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class HiddenWhenTrue : BoolToVisibility
	{
		public HiddenWhenTrue()
		{
			FalseValue = Visibility.Visible;
			TrueValue = Visibility.Hidden;
		}
	}

	public class CollapsedWhenFalse : BoolToVisibility
	{
		public CollapsedWhenFalse()
		{
			FalseValue = Visibility.Collapsed;
			TrueValue = Visibility.Visible;
		}
	}

	public class HiddenWhenFalse : BoolToVisibility
	{
		public HiddenWhenFalse()
		{
			FalseValue = Visibility.Hidden;
			TrueValue = Visibility.Visible;
		}
	}
}
