using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace XTestMan.Utils
{
	public static class BaseExtensions
	{


		public static int GetMyInt(DependencyObject obj)
		{
			return (int)obj.GetValue(MyIntProperty);
		}

		public static void SetMyInt(DependencyObject obj, int value)
		{
			obj.SetValue(MyIntProperty, value);
		}

		// Using a DependencyProperty as the backing store for MyInt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MyIntProperty =
			DependencyProperty.RegisterAttached("MyInt", typeof(int), typeof(BaseExtensions), new PropertyMetadata(0, OnMyIntChanged));

		private static void OnMyIntChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine($"I has changed from {e.OldValue} to {e.NewValue}");
		}



		public static bool GetIsScrollableValue(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsScrollableValueProperty);
		}

		public static void SetIsScrollableValue(DependencyObject obj, bool value)
		{
			obj.SetValue(IsScrollableValueProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsScrollableValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsScrollableValueProperty =
			DependencyProperty.RegisterAttached("IsScrollableValue", typeof(bool), typeof(BaseExtensions), new PropertyMetadata(false, OnIsScrollableValueChanged));

		private static void OnIsScrollableValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as UIElement).MouseWheel += OnScrolling;
		}

		private static void OnScrolling(object sender, MouseWheelEventArgs e)
		{
			var val = GetMyInt(sender as DependencyObject);

			Debug.WriteLine($"old scroll {val}");
			val += Math.Sign(e.Delta);
			Debug.WriteLine($"new Scrolling {val}");
			SetMyInt(sender as DependencyObject, val);
		}



		public static bool GetIsDragValueChange(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsDragValueChangeProperty);
		}

		public static void SetIsDragValueChange(DependencyObject obj, bool value)
		{
			obj.SetValue(IsDragValueChangeProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsDragValueChange.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsDragValueChangeProperty =
			DependencyProperty.RegisterAttached("IsDragValueChange", typeof(bool), typeof(BaseExtensions), new PropertyMetadata(false, OnIsDragChanged));
		private static double _lastX;

		private static void OnIsDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var item = d as UIElement;
			if (item != null)
			{
				item.PreviewMouseDown += Startdrag;
				item.PreviewMouseUp += Enddrag;
				item.PreviewMouseMove += OnDrag;
			}
		}

		private static void OnDrag(object sender, MouseEventArgs e)
		{
			var item = sender as UIElement;
			if (item != null)
			{
				if (item.IsMouseCaptured)
				{
					var newX = e.GetPosition(null).X;
					var delta = newX - _lastX;
					var d = item as DependencyObject;
					var value = GetMyInt(d);
					SetMyInt(d, value + Math.Sign(delta));
					_lastX = newX;
				}
			}

		}

		private static void Enddrag(object sender, MouseButtonEventArgs e)
		{
			var item = sender as UIElement;
			if (item != null)
			{
				item.ReleaseMouseCapture();
			}
		}

		private static void Startdrag(object sender, MouseButtonEventArgs e)
		{
			var item = sender as UIElement;
			if (item != null)
			{
				item.CaptureMouse();
			}
		}




		public static UIElement GetElementToFocus(DependencyObject obj)
		{
			return (UIElement)obj.GetValue(ElementToFocusProperty);
		}

		public static void SetElementToFocus(DependencyObject obj, UIElement value)
		{
			obj.SetValue(ElementToFocusProperty, value);
		}

		// Using a DependencyProperty as the backing store for ElementToFocus.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ElementToFocusProperty =
			DependencyProperty.RegisterAttached("ElementToFocus", typeof(UIElement), typeof(BaseExtensions), new PropertyMetadata());



		public static bool GetIsRedirectFocus(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsRedirectFocusProperty);
		}

		public static void SetIsRedirectFocus(DependencyObject obj, bool value)
		{
			obj.SetValue(IsRedirectFocusProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsRedirectFocus.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsRedirectFocusProperty =
			DependencyProperty.RegisterAttached("IsRedirectFocus", typeof(bool), typeof(BaseExtensions), new PropertyMetadata(false, OnRedirectFocusChanged));

		private static void OnRedirectFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var item = d as UIElement;
			if (item != null)
			{
				item.GotFocus += OnGotFocus;
			}
		}

		private static void OnGotFocus(object sender, RoutedEventArgs e)
		{
			var focusedElement = sender as DependencyObject;
			if (focusedElement != null)
			{
				var elementToFocus = GetElementToFocus(focusedElement);
				elementToFocus.Focus();
			}
		}
	}
}
