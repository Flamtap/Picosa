using System;
using System.Windows.Input;
using Picosa.App.Infrastructure;

namespace Picosa.App.Features
{
    public class PhotoEditorViewModel : ViewModelBase
    {
        public ICommand OpenCommand => new RelayCommand(Open);

        private void Open()
        {
            throw new NotImplementedException();
        }

        public ICommand SaveCommand => new RelayCommand(Save);

        private void Save()
        {
            throw new NotImplementedException();
        }

        public ICommand SaveAsCommand => new RelayCommand(SaveAs);

        private void SaveAs()
        {
            throw new NotImplementedException();
        }

        public ICommand CropCommand => new RelayCommand(Crop);

        private void Crop()
        {
            throw new NotImplementedException();
        }

        public ICommand AddWatermarkCommand => new RelayCommand(AddWatermark);

        private void AddWatermark()
        {
            throw new NotImplementedException();
        }
    }
}
