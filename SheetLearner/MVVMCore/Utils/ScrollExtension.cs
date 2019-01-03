using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMCore.Utils
{
    public class ScrollExtension
    { 
        public static int GetScrollableValue(DependencyObject obj)
        {
            return (int)obj.GetValue(ScrollableValueProperty);
        }

        public static void SetScrollableValue(DependencyObject obj, int value)
        {
            obj.SetValue(ScrollableValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollableValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollableValueProperty =
            DependencyProperty.RegisterAttached("ScrollableValue", typeof(int), typeof(ScrollExtension), new FrameworkPropertyMetadata(0,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnMyValueChanges));

        private static void OnMyValueChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public static bool GetDoScrollValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(DoScrollValueProperty);
        }

        public static void SetDoScrollValue(DependencyObject obj, bool value)
        {
            obj.SetValue(DoScrollValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for DoScrollValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoScrollValueProperty =
            DependencyProperty.RegisterAttached("DoScrollValue", typeof(bool), typeof(ScrollExtension), new PropertyMetadata(false,DoScrollValueChange));

        private static void DoScrollValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scv = GetScrollableValue(d);
            if(scv != null)
            {
                var item = d as UIElement;
                if(item != null)
                {
                    if((bool)e.NewValue)
                        item.MouseWheel += DoMouseWheelInc;
                    else
                        item.MouseWheel -= DoMouseWheelInc;
                }

            }
        }

        private static void DoMouseWheelInc(object sender, MouseWheelEventArgs e)
        {
            var val = GetScrollableValue(sender as DependencyObject);
            if(val != null)
            {
                val += Math.Sign(e.Delta);
                SetScrollableValue(sender as DependencyObject, val);
            }
        } 
    }
}
