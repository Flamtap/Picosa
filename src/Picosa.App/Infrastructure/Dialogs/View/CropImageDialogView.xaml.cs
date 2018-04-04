using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Picosa.App.Infrastructure.Dialogs.ViewModel;
using Picosa.Drawing;
using Picosa.UI;

namespace Picosa.App.Infrastructure.Dialogs.View
{
    /// <summary>
    /// Interaction logic for CropImageDialogView.xaml
    /// </summary>
    public partial class CropImageDialogView
    {
        private CroppingAdorner _croppingAdorner;

        public CropImageDialogView()
        {
            InitializeComponent();

            MainImage.Loaded += OnMainImageLoaded;
        }

        private void OnMainImageLoaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(_croppingAdorner == null);

            if (!(DataContext is CropImageDialogViewModel viewModel))
                return;

            if (viewModel.OriginalImage.PixelWidth < MainImage.MaxWidth &&
                viewModel.OriginalImage.PixelHeight < MainImage.MaxHeight)
            {
                MainImage.Stretch = Stretch.None;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(MainImage);

            _croppingAdorner = new CroppingAdorner(MainImage)
            {
                ForceSquare = false,
                CroppingUnitType = CroppingUnitType.BitmapPixels,
                MinimumSelectionHeight = viewModel.MinimumCropHeight,
                MinimumSelectionWidth = viewModel.MinimumCropWidth
            };

            _croppingAdorner.SelectionChanged += (s, a) => UpdateCurrentImage();

            adornerLayer.Add(_croppingAdorner);

            if (viewModel.OriginalImage.PixelHeight > viewModel.MinimumCropHeight
                && viewModel.OriginalImage.PixelWidth > viewModel.MinimumCropWidth)
                UpdateCurrentImage();
        }

        private void UpdateCurrentImage()
        {
            if (!(DataContext is CropImageDialogViewModel viewModel))
                return;

            viewModel.CurrentImage = _croppingAdorner.GetCroppedBitmapSource();
            viewModel.CropArea = _croppingAdorner.Selection.ToRectangle();
        }
    }
}
