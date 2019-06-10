using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MVVMCore.Utils
{
	public class MouseMovable
	{

		[Flags]
		public enum Axis
		{
			HORIZONTAL = 1,
			VERTICAL,
			BOTH
		}



		public static Axis GetAxis(DependencyObject obj)
		{
			return (Axis)obj.GetValue(AxisProperty);
		}

		public static void SetAxis(DependencyObject obj, Axis value)
		{
			obj.SetValue(AxisProperty, value);
		}

		// Using a DependencyProperty as the backing store for Axis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AxisProperty =
			DependencyProperty.RegisterAttached("Axis", typeof(Axis), typeof(MouseMovable), new PropertyMetadata(Axis.BOTH));

		public static bool GetIsMouseMovable(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsMouseMovableProperty);
		}

		public static void SetIsMouseMovable(DependencyObject obj, bool value)
		{
			obj.SetValue(IsMouseMovableProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsMouseMovable.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsMouseMovableProperty =
			DependencyProperty.RegisterAttached("IsMouseMovable", typeof(bool), typeof(MouseMovable), new PropertyMetadata(false, MouseMovableChanged));
		private static double _lastPosX;
		private static double _lastPosY;

		private static void MouseMovableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			{
				var item = d as UIElement;
				if (item != null)
				{
					if ((bool)e.NewValue)
					{
						item.PreviewMouseDown += OnMouseDown;
						item.MouseMove += OnMouseMove;
						item.MouseUp += OnMouseUp;
					}
					else
					{
						item.PreviewMouseDown -= OnMouseDown;
						item.MouseMove -= OnMouseMove;
						item.MouseUp -= OnMouseUp;
					}
				}
			}
		}



		public static Action<object, Point> GetMoveCallback(DependencyObject obj)
		{
			return (Action<object, Point>)obj.GetValue(MoveCallbackProperty);
		}

		public static void SetMoveCallback(DependencyObject obj, Action<object, Point> value)
		{
			obj.SetValue(MoveCallbackProperty, value);
		}

		// Using a DependencyProperty as the backing store for MoveCallback.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MoveCallbackProperty =
			DependencyProperty.RegisterAttached("MoveCallback", typeof(Action<object, Point>), typeof(MouseMovable), new PropertyMetadata(null));


		private static void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			var item = sender as FrameworkElement;
			item.ReleaseMouseCapture();
		}

		private static void OnMouseMove(object sender, MouseEventArgs e)
		{
			void move(FrameworkElement fi, Axis restriction, double x, double y)
			{
				//if (restriction.HasFlag(Axis.VERTICAL) || restriction.HasFlag(Axis.BOTH))
				//    Canvas.SetTop(fi, y);

				//if (restriction.HasFlag(Axis.HORIZONTAL) || restriction.HasFlag(Axis.BOTH))
				//    Canvas.SetLeft(fi, x);

				Debug.WriteLine("moving");
			}

			var item = sender as FrameworkElement;
			var axis = GetAxis(sender as DependencyObject);
			if (item.IsMouseCaptured)
			{
				var root = GetCanvasRoot(item);// as IInputElement;
				var nx = e.GetPosition(root).X;
				var ny = e.GetPosition(root).Y;

				var dx = nx - _lastPosX;
				var dy = ny - _lastPosY;

				//var px = nx / root.ActualWidth;
				//var py = ny / root.ActualHeight;
				//var left = Canvas.GetLeft(item); // dont get top
				//var top = Canvas.GetTop(item); 

				if (GetMoveCallback(sender as DependencyObject) != null)
				{
					var callback = GetMoveCallback(sender as DependencyObject);
					callback(item.DataContext, new Point(nx, ny));

					var res = GetMyDistance(sender as DependencyObject)?.Invoke(item.DataContext, new Point(nx, ny));
					if (res != null)
					{
						res = Math.Abs((float)res);
						var d = GetDistanceTreshold(sender as DependencyObject);
						if (d > res)
						{
							move(item, axis, nx, ny);
						}
					}
					else
					{
						move(item, axis, nx, ny);
					}
				}
				else
				{
					move(item, axis, nx, ny);
				}

				_lastPosX = nx;
				_lastPosY = ny;
			}
		}

		private static void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var item = sender as FrameworkElement;
			item.CaptureMouse();
		}



		public static Func<object, Point, float> GetMyDistance(DependencyObject obj)
		{
			return (Func<object, Point, float>)obj.GetValue(MyDistanceProperty);
		}

		public static void SetMyDistance(DependencyObject obj, Func<object, Point, float> value)
		{
			obj.SetValue(MyDistanceProperty, value);
		}

		// Using a DependencyProperty as the backing store for MyDistance.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MyDistanceProperty =
			DependencyProperty.RegisterAttached("MyDistance", typeof(Func<object, Point, float>), typeof(MouseMovable), new PropertyMetadata(null));



		public static float GetDistanceTreshold(DependencyObject obj)
		{
			return (float)obj.GetValue(DistanceTresholdProperty);
		}

		public static void SetDistanceTreshold(DependencyObject obj, float value)
		{
			obj.SetValue(DistanceTresholdProperty, value);
		}

		// Using a DependencyProperty as the backing store for DistanceTreshold.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DistanceTresholdProperty =
			DependencyProperty.RegisterAttached("DistanceTreshold", typeof(float), typeof(MouseMovable), new PropertyMetadata(null));



		public static FrameworkElement GetCanvasRoot(DependencyObject obj)
		{
			return (FrameworkElement)obj.GetValue(CanvasRootProperty);
		}

		public static void SetCanvasRoot(DependencyObject obj, FrameworkElement value)
		{
			obj.SetValue(CanvasRootProperty, value);
		}

		// Using a DependencyProperty as the backing store for CanvasRoot.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CanvasRootProperty =
			DependencyProperty.RegisterAttached("CanvasRoot", typeof(FrameworkElement), typeof(MouseMovable), new PropertyMetadata(Changed));

		private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}
	}
}
