using System;
using System.Windows;

namespace Picosa.App.Infrastructure.Dialogs
{
    public static class Dialog
    {
        public static bool? Show(DialogViewModel dialogViewModel)
        {
            if (dialogViewModel == null)
                throw new ArgumentNullException(nameof(dialogViewModel));

            var window = new DialogWindow(dialogViewModel) { Owner = Application.Current.MainWindow };

            return window.ShowDialog();
        }

        public static void ShowError(string mainText, string caption)
        {
            MessageBox.Show(mainText, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
