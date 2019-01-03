using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XTestMan.ViewModels;

namespace XTestMan.Utils
{
    class EditableTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate EditableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var val = ((EditableNumberedItemViewModel)item).IsEditing;
            if (val)
                return EditableTemplate;

            return DefaultTemplate;
        }
    }
}
