using System.Windows;
using System.Windows.Controls;

namespace XTestMan.Views.Music
{
	/// <summary>
	/// Interaction logic for PlayableNote.xaml
	/// </summary>
	public partial class PlayableNote : UserControl
	{
		public PlayableNote()
		{
			InitializeComponent();
		}



		public bool IsPlayed
		{
			get { return (bool)GetValue(IsPlayedProperty); }
			set { SetValue(IsPlayedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsPlayed.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsPlayedProperty =
			DependencyProperty.Register("IsPlayed", typeof(bool), typeof(PlayableNote), new PropertyMetadata(false));


	}
}
