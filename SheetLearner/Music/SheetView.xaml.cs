using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XTestMan.Views.Music
{
    /// <summary>
    /// Interaction logic for SheetView.xaml
    /// </summary>
    public partial class SheetView : UserControl
    {
        public SheetView()
        {
            InitializeComponent();
            DataContext = new SheetViewModel();
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
