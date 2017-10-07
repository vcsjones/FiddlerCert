using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace VCSJones.FiddlerCert
{
    public static class DoubleClickCopyBehavior
    {
        public static readonly DependencyProperty IsCopyToClipBoardOnDoubleClickProperty =
            DependencyProperty.RegisterAttached("IsCopyToClipBoardOnDoubleClick", typeof(bool), typeof(DoubleClickCopyBehavior), new PropertyMetadata { DefaultValue = false, PropertyChangedCallback = OnIsCopyToClipBoardOnDoubleClickChanged });

        public static readonly DependencyProperty CopyToClipBoardOnDoubleClickTextProperty =
            DependencyProperty.RegisterAttached("CopyToClipBoardOnDoubleClickText", typeof(string), typeof(DoubleClickCopyBehavior), new PropertyMetadata { DefaultValue = null });

        public static readonly DependencyProperty LastClickTimeProperty =
            DependencyProperty.RegisterAttached("LastClickTime", typeof(int?), typeof(DoubleClickCopyBehavior), new PropertyMetadata { DefaultValue = null });

        public static readonly DependencyProperty LastClickLocationProperty =
            DependencyProperty.RegisterAttached("LastClickLocation", typeof(Point?), typeof(DoubleClickCopyBehavior), new PropertyMetadata { DefaultValue = null });

        public static bool GetIsCopyToClipBoardOnDoubleClick(UIElement element)
        {
            return (bool)element.GetValue(IsCopyToClipBoardOnDoubleClickProperty);
        }

        public static void SetIsCopyToClipBoardOnDoubleClick(UIElement element, bool value)
        {
            element.SetValue(IsCopyToClipBoardOnDoubleClickProperty, value);
        }

        public static string GetCopyToClipBoardOnDoubleClickText(UIElement element)
        {
            return (string)element.GetValue(IsCopyToClipBoardOnDoubleClickProperty);
        }

        public static void SetCopyToClipBoardOnDoubleClickText(UIElement element, string value)
        {
            element.SetValue(CopyToClipBoardOnDoubleClickTextProperty, value);
        }

        public static void OnIsCopyToClipBoardOnDoubleClickChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var element = sender as UIElement;
            if (element == null)
            {
                return;
            }
            bool enabled = (bool)args.NewValue;
            if (enabled)
            {
                element.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
                element.GotFocus += UIElement_GotFocus;
            }
            else
            {
                element.MouseLeftButtonUp -= UIElement_MouseLeftButtonUp;
                element.GotFocus -= UIElement_GotFocus;
            }
        }

        private static void UIElement_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            ClearDoubleClickProperties(element);
        }

        private static void ClearDoubleClickProperties(UIElement element)
        {
            element.SetValue(LastClickLocationProperty, null);
            element.SetValue(LastClickTimeProperty, null);
        }

        private static void UIElement_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            if (element == null)
            {
                return;
            }
            var lastClickLocation = element.GetValue(LastClickLocationProperty) as Point?;
            var lastClickTime = element.GetValue(LastClickTimeProperty) as int?;
            var copyText = element.GetValue(CopyToClipBoardOnDoubleClickTextProperty) as string;
            if (lastClickLocation.HasValue && lastClickTime.HasValue)
            {
                if (e.Timestamp - lastClickTime.Value > SystemInformation.DoubleClickTime)
                {
                    element.SetValue(LastClickLocationProperty, e.GetPosition(element));
                    element.SetValue(LastClickTimeProperty, e.Timestamp);
                    return;
                }
                var size = SystemInformation.DoubleClickSize;
                var rect = new Rect(lastClickLocation.Value, lastClickLocation.Value);
                rect.Inflate(size.Width / 2, size.Height / 2);
                if (rect.Contains(e.GetPosition(element)))
                {
                    if (string.IsNullOrEmpty(copyText))
                    {
                        if (element is ContentControl contentControl)
                        {
                            Debug.WriteLine($"Clipboard set to: \"{contentControl.Content.ToString()}\"");
                            System.Windows.Clipboard.SetText(contentControl.Content.ToString());
                        }
                        if (element is TextBlock textBlock)
                        {
                            Debug.WriteLine($"Clipboard set to: \"{textBlock.Text}\"");
                            System.Windows.Clipboard.SetText(textBlock.Text);
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Clipboard set to: \"{copyText}\"");
                        System.Windows.Clipboard.SetText(copyText);
                    }
                    ClearDoubleClickProperties(element);
                    return;

                }
            }
            element.SetValue(LastClickLocationProperty, e.GetPosition(element));
            element.SetValue(LastClickTimeProperty, e.Timestamp);
        }
    }
}
