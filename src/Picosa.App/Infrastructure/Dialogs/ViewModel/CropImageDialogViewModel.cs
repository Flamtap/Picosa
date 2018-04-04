using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Picosa.App.Infrastructure.Dialogs.ViewModel
{
    public class CropImageDialogViewModel : DialogViewModel
    {
        private BitmapImage _originalImage;
        private BitmapSource _currentImage;

        public CropImageDialogViewModel(string sourceFileName) =>
            OriginalImage = new BitmapImage(new Uri(sourceFileName));

        public int MinimumCropWidth => 64;

        public int MinimumCropHeight => 64;

        public Rectangle CropArea { get; set; }

        public BitmapImage OriginalImage
        {
            get => _originalImage;
            set
            {
                _originalImage = value;

                OnPropertyChanged();
            }
        }

        public BitmapSource CurrentImage
        {
            get => _currentImage;
            set
            {
                if (Equals(_currentImage, value))
                    return;

                _currentImage = value;
                OnPropertyChanged();
            }
        }
    }
}
