using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SheetViews.Music
{
	/// <summary>
	/// Interaction logic for NoteView.xaml
	/// </summary>
	public partial class NoteView : UserControl
	{
		public NoteView()
		{
			InitializeComponent();
			Loaded += NoteView_Loaded;
		}

		static Random rand;
		private void NoteView_Loaded(object sender, RoutedEventArgs e)
		{
			rand = rand ?? new Random();
			var time = rand.NextDouble() * 2000.0;
			time = Math.Min(400, time);
			//AnimateRenderY(-300,TimeSpan.FromMilliseconds(time));
			AnimateRenderY(rand.NextDouble() * -200, TimeSpan.FromMilliseconds(time));

			(DataContext as INotifyPropertyChanged).PropertyChanged += NoteView_PropertyChanged;
		}

		private void NoteView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Played")
			{
				Dispatcher.Invoke(() =>
				{
					AnimateRenderY(-20, TimeSpan.FromMilliseconds(200));
				});
			}
		}

		private void AnimateRenderY(double from, TimeSpan time)
		{
			var storyBoard = new Storyboard();
			var fallAnimation = new DoubleAnimation
			{
				From = from,
				Duration = new Duration(time)
			};
			Storyboard.SetTargetProperty(fallAnimation, new PropertyPath("(TranslateTransform.Y)"));
			Storyboard.SetTargetName(fallAnimation, "pos");
			storyBoard.Children.Add(fallAnimation);

			storyBoard.Begin(this);
		}

		public void AnimatePlay()
		{
			AnimateRenderY(20, TimeSpan.FromMilliseconds(300));
		}

		private void Elp_MouseEnter(object sender, MouseEventArgs e)
		{
			AnimatePlay();
		}
	}
}
