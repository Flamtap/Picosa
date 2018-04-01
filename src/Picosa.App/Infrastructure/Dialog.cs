using System.Windows;

namespace Picosa.App.Infrastructure
{
    public static class Dialog
    {
        public static void ShowError(string mainText, string caption)
        {
            MessageBox.Show(mainText, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
