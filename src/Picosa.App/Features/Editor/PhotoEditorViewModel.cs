using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Picosa.App.Infrastructure;

namespace Picosa.App.Features.Editor
{
    public class PhotoEditorViewModel : ViewModelBase
    {
        private readonly string _fileName;

        public PhotoEditorViewModel(string fileName, BitmapImage originalImage)
        {
            _fileName = fileName;
            OriginalImage = originalImage;
        }
        
        public string FileName => _fileName;

        public BitmapImage OriginalImage { get; }

        #region Commands

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

        #endregion
    }
}
