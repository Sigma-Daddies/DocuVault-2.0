using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DocuVault.Utils
{
    public class ToastNotifier
    {
        private readonly StackPanel _toasterPanel;

        public ToastNotifier(StackPanel toasterPanel)
        {
            _toasterPanel = toasterPanel;
        }

        public async Task ShowToastWarning(string message, string backgroundColor = "#982929", int durationInSeconds = 3)
        {
            // Create a border to hold the notification
            var toast = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFromString(backgroundColor),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(10)
            };

            // Create the text for the notification
            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            toast.Child = textBlock;

            // Add the toast to the panel
            _toasterPanel.Children.Add(toast);

            // Wait for the duration, then remove the toast
            await Task.Delay(durationInSeconds * 1000);
            _toasterPanel.Children.Remove(toast);
        }        
        public async Task ShowToastConfirm(string message, string backgroundColor = "#2B5636", int durationInSeconds = 3)
        {
            // Create a border to hold the notification
            var toast = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFromString(backgroundColor),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(10)
            };

            // Create the text for the notification
            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            toast.Child = textBlock;

            // Add the toast to the panel
            _toasterPanel.Children.Add(toast);

            // Wait for the duration, then remove the toast
            await Task.Delay(durationInSeconds * 1000);
            _toasterPanel.Children.Remove(toast);
        }
    }
}
