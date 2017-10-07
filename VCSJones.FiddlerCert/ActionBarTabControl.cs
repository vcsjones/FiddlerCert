using System.Windows;
using System.Windows.Controls;

namespace VCSJones.FiddlerCert
{
    internal class ActionBarTabControl : TabControl
    {
        internal static DependencyProperty ActionBarTemplateProperty = DependencyProperty.Register(nameof(ActionBarTemplate), typeof(ContentControl), typeof(ActionBarTabControl), new FrameworkPropertyMetadata((DataTemplate)null));

        public ContentControl ActionBarTemplate
        {
            get => (ContentControl)GetValue(ActionBarTemplateProperty);
            set => SetValue(ActionBarTemplateProperty, value);
        }
    }
}
