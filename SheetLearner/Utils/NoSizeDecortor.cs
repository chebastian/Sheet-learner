using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace XTestMan.Utils
{
    public class NoSizeDecorator : Decorator
    {
        protected override Size MeasureOverride(Size constraint)
        {
            Child.Measure(new Size(0,0));
            return new Size(0,0);
        } 
    }

    public class SharpAdorner : Adorner
    {
        public SharpAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Color = new SolidColorBrush(Colors.Red);
        }

        public SolidColorBrush Color { get; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var rect = AdornedElement.DesiredSize;
            var text = new TextBlock() { Text = "#" , Foreground=Color};

            drawingContext.DrawText(new FormattedText("#", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, Color),new Point());
            base.OnRender(drawingContext);
        }
    }
}
