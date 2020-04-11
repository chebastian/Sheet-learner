using Music.Models;
using System.Windows;
using System.Windows.Controls;

namespace SheetViews.Music
{
	/// <summary>
	/// Interaction logic for SheetView.xaml
	/// </summary>
	public partial class SheetView : UserControl
	{
		public SheetView()
		{
			InitializeComponent();
			//DataContext = new SheetViewModel();
		}



		public Clef CurrentClef
		{
			get { return (Clef)GetValue(CurrentClefProperty); }
			set { SetValue(CurrentClefProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentClef.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentClefProperty =
			DependencyProperty.Register("CurrentClef", typeof(Clef), typeof(SheetView), new PropertyMetadata(Clef.Treble));

	}
}
