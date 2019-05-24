using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SheetLearner.Utils
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

	public class CollapsedWhenTrue : BoolToVisibility
	{
		public CollapsedWhenTrue()
		{
			TrueValue = Visibility.Collapsed;
			FalseValue = Visibility.Visible;
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

	public class HalfValue : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((double)value) / 2;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value) * 2;
		}
	}
}

