using System.Windows;
using System.Windows.Controls;

namespace SheetViews.TabNavigator
{
	/// <summary>
	/// Interaction logic for NavigationPane.xaml
	/// </summary>
	public partial class NavigationPane : UserControl
	{
		public NavigationPane()
		{
			InitializeComponent();
		}




		public DataTemplateSelector DataTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
			set { SetValue(DataTemplateSelectorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataTemplateSelector.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataTemplateSelectorProperty =
			DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationPane), new PropertyMetadata(null));
	}
}
