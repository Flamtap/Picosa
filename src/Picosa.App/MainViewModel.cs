namespace Picosa.App
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public MainViewModel()
            : this(null)
        {
        }

        public MainViewModel(ViewModelBase currentViewModel) => _currentViewModel = currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;

                OnPropertyChanged();
            }
        }
    }
}
