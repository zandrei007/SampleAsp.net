using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ConfigParser.TemplateSelectors
{
    public class AttributeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var elemnt = container as FrameworkElement;

            if (elemnt == null)
            {
                return null;
            }

            if (MainWindow.DiffAttr.Contains(item))
            {
                return elemnt.FindResource("DiffDataTemplate") as DataTemplate;
            }

            if (MainWindow.ChangedAttr.Contains(item))
            {
                return elemnt.FindResource("ChangedDataTemplate") as DataTemplate;
            }

            return elemnt.FindResource("NormalUserDataTemplate") as DataTemplate;
        }
    }
}
