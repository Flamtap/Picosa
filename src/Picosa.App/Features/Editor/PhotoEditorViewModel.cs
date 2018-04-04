using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Picosa.App.Infrastructure;
using Picosa.App.Infrastructure.Dialogs;
using Picosa.App.Infrastructure.Dialogs.ViewModel;
using Picosa.Util;

namespace Picosa.App.Features.Editor
{
    public class PhotoEditorViewModel : ViewModelBase
    {
        private BitmapImage _currentImage;
        private bool _isDirty;

        public PhotoEditorViewModel(string fileName, BitmapImage currentImage)
        {
            FileName = fileName;
            _currentImage = currentImage;

        }
        
        public string FileName { get; }

        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;

                OnPropertyChanged();
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;

                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand CropCommand => new RelayCommand(Crop);

        private void Crop()
        {
            var cropViewModel = new CropImageDialogViewModel(FileName);

            if (Dialog.Show(cropViewModel) != true)
                return;

            var cropArea = cropViewModel.CropArea;

            var sourceBmp = new Bitmap(FileName);
            var newBmp = new Bitmap(cropArea.Width, cropArea.Height);

            var graphics = Graphics.FromImage(newBmp);
            graphics.DrawImage(sourceBmp, -cropArea.X, -cropArea.Y);

            CurrentImage = BitmapConverter.ToBitmapImage(newBmp);
        }

        public ICommand AddWatermarkCommand => new RelayCommand(AddWatermark);

        private void AddWatermark()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
