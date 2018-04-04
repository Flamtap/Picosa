using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Picosa.App.Infrastructure;
using Picosa.App.Infrastructure.Dialogs;
using Picosa.App.Infrastructure.Dialogs.ViewModel;

namespace Picosa.App.Features.Editor
{
    public class PhotoEditorViewModel : ViewModelBase
    {
        private bool _isDirty;
        private BitmapImage _currentImage;

        public PhotoEditorViewModel(string fileName, BitmapImage currentImage)
        {
            FileName = fileName;
            CurrentImage = currentImage;
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

            if (Dialog.Show(cropViewModel) == true)
                CurrentImage = cropViewModel.GetCroppedImage();
        }

        public ICommand AddWatermarkCommand => new RelayCommand(AddWatermark);

        private void AddWatermark()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
