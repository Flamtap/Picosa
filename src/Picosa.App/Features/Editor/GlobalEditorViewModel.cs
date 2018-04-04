using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Picosa.App.Infrastructure;
using Picosa.App.Infrastructure.Dialogs;

namespace Picosa.App.Features.Editor
{
    public class GlobalEditorViewModel : ViewModelBase
    {
        private PhotoEditorViewModel _currentEditor;

        public PhotoEditorViewModel CurrentEditor
        {
            get => _currentEditor;
            set
            {
                _currentEditor = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEditor));
            }
        }

        public bool HasEditor => CurrentEditor != null;

        #region Commands

        public ICommand OpenCommand => new RelayCommand(Open);

        private void Open()
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Select an image...",
                Filter = "All Picture Files|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All Files|*.*"
            };

            if (openDialog.ShowDialog() == true && TryLoadImage(openDialog.FileName, out var image))
                CurrentEditor = new PhotoEditorViewModel(openDialog.FileName, image);
        }

        private static bool TryLoadImage(string fileName, out BitmapImage image)
        {
            image = null;

            try
            {
                image = new BitmapImage(new Uri(fileName));
            }
            catch (DirectoryNotFoundException)
            {
                Dialog.ShowError("The directory was not found, or no longer exists.", "Can't load image");
                return false;
            }
            catch (FileNotFoundException)
            {
                Dialog.ShowError("The file was not found, or no longer exists.", "Can't load image");
                return false;
            }
            catch (IOException)
            {
                Dialog.ShowError("The file could not be read.", "Can't load image");
                return false;
            }
            catch (NotSupportedException)
            {
                Dialog.ShowError("The image file format is not supported.", "Can't load image");
                return false;
            }

            return true;
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

        #endregion
    }
}
