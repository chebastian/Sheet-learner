using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SheetLearner.Music
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

			(DataContext as NoteViewModel).PropertyChanged += NoteView_PropertyChanged;
		}

		private void NoteView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Played")
			{
				Dispatcher.Invoke(() =>
				{
					AnimateRenderY(20, TimeSpan.FromMilliseconds(200));
				});
			}
		}

		private async void AnimateRenderY(double from, TimeSpan time)
		{
			var storyBoard = new Storyboard();

			var tanim = new ThicknessAnimation();
			tanim.From = new Thickness(0, 0, 0, 0);
			tanim.By = new Thickness(0, 0, 0, 20);
			tanim.To = new Thickness(0, 0, 0, -20);
			tanim.Duration = new Duration(TimeSpan.FromMilliseconds(40));
			var danim = new DoubleAnimation();
			danim.From = from;
			//danim.By = 20;
			//danim.To = 20;
			danim.Duration = new Duration(time);
			Storyboard.SetTargetProperty(danim, new PropertyPath("(TranslateTransform.Y)"));
			Storyboard.SetTargetName(danim, "pos");
			storyBoard.Children.Add(danim);

			storyBoard.Begin(this);
			await Task.Delay(1000);
		}

		public async void AnimatePlay()
		{
			AnimateRenderY(20, TimeSpan.FromMilliseconds(300));
		}

		private void Elp_MouseEnter(object sender, MouseEventArgs e)
		{
			AnimatePlay();
		}
	}
}
