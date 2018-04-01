namespace Picosa.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(MainViewModel viewModel = null)
        {
            DataContext = viewModel ?? new MainViewModel();

            InitializeComponent();
        }
    }
}
