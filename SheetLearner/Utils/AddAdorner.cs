using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace XTestMan.Utils
{
    public class AddAdorner
    {


        public static bool GetIsSharp(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSharpProperty);
        }

        public static void SetIsSharp(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSharpProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSharp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSharpProperty =
            DependencyProperty.RegisterAttached("IsSharp", typeof(bool), typeof(AddAdorner), new PropertyMetadata(false,OnChanged));

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is FrameworkElement elem)
            {
                bool isSet = (bool)e.NewValue;
                if(isSet)
                    elem.Loaded += Elem_Loaded;
                else
                { 
                    elem.Loaded -= Elem_Loaded;
                    RemoveAdornerOnElement(elem);
                }
            }
        }

        private static void RemoveAdornerOnElement(FrameworkElement elem)
        {
            var layer = AdornerLayer.GetAdornerLayer(elem);
            if (layer == null)
                return;

            var adorners = layer.GetAdorners(elem);
            foreach(var adorner in adorners)
            {
                layer.Remove(adorner);
            }
        }

        private static void Elem_Loaded(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement elem)
            {
                var layer = AdornerLayer.GetAdornerLayer(elem);
                    if (layer == null)
                        return;

                layer.Add(new SharpAdorner(elem)); 
            }
        }
    }
}
