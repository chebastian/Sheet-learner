using System.Windows;
using System.Windows.Controls;

namespace SheetLearner.Utils
{
	public class PageTemplateSelector : DataTemplateSelector
	{
		public DataTemplate FirstView { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return FirstView;
		}
	}
}
