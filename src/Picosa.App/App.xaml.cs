using System.Windows;
using System.Windows.Threading;
using Picosa.App.Common;
using Picosa.App.Features;

namespace Picosa.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainViewModel(new PhotoEditorViewModel());
            var mainWindow = new MainWindow(viewModel);

            mainWindow.Show();

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var viewModel = new UnhandledExceptionViewModel(e.Exception);
            var window = new Window
            {
                Title = "Picosa - Error",
                Height = 400,
                Width = 500,
                SizeToContent = SizeToContent.Manual,
                DataContext = viewModel,
                Content = viewModel,
                ContentTemplate = new DataTemplate(viewModel.GetType())
                {
                    VisualTree = new FrameworkElementFactory(typeof(UnhandledExceptionView))
                }
            };

            viewModel.PropertyChanged += (propSender, args) =>
            {
                if (viewModel.DialogResult.HasValue)
                    window.Close();
            };

            window.ShowDialog();
            e.Handled = true;
        }
    }
}
