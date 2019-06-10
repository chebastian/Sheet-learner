using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MVVMCore.Converters
{
	public class VisibilityConverter : IValueConverter
	{
		public Visibility TrueValue { get; set; }
		public Visibility FalseValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (((bool)value))
			{
				return TrueValue;
			}

			else return FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
