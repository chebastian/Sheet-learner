using SharedLibraries.Interfaces;
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

namespace SheetViews.TabNavigator
{
    /// <summary>
    /// Interaction logic for NavigationPane.xaml
    /// </summary>
    public partial class NavigationPane : UserControl
    {
        public NavigationPane()
        {
            InitializeComponent();
        }




        public DataTemplateSelector DataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
            set { SetValue(DataTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationPane), new PropertyMetadata(null)); 
    }
}
