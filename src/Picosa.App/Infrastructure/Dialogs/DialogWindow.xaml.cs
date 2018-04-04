using System;
using System.Windows.Input;

namespace Picosa.App.Infrastructure.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow
    {
        public DialogWindow(DialogViewModel dialogViewModel)
        {
            DataContext = dialogViewModel;

            InitializeComponent();
        }

        private void HandleContentRendered(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
