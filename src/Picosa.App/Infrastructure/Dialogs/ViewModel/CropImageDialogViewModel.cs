using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Picosa.App.Infrastructure.Dialogs.ViewModel
{
    public class CropImageDialogViewModel : DialogViewModel
    {
        private readonly string _sourceFileName;
        private BitmapImage _originalImage;
        private BitmapSource _currentImage;

        public CropImageDialogViewModel(BitmapImage originalImage) => _originalImage = originalImage;

        public CropImageDialogViewModel(string sourceFileName)
        {
            _sourceFileName = sourceFileName;
            OriginalImage = new BitmapImage(new Uri(_sourceFileName));
        }

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

        public Rectangle CropArea { get; set; }

        public int MinimumCropWidth => 64;

        public int MinimumCropHeight => 64;

        public BitmapImage GetCroppedImage()
        {
            var sourceBmp = new Bitmap(_sourceFileName);

            var newBmp = new Bitmap(CropArea.Width, CropArea.Height);
            var graphics = Graphics.FromImage(newBmp);
            graphics.DrawImage(sourceBmp, -CropArea.X, -CropArea.Y);

            return BitmapConverter.ToBitmapImage(newBmp);
        }
    }
}
