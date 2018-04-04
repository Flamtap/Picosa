using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Picosa.UI
{
    public enum CroppingUnitType
    {
        DeviceIndependentPixels,
        BitmapPixels
    }

    public class CroppingAdorner : Adorner
    {
        #region Fields

        private static readonly Brush DefaultMask = new SolidColorBrush(Color.FromArgb(0x7f, 0, 0, 0));

        private const int ThumbSize = 6;

        private readonly VisualCollection _visualCollection;

        private readonly Thumb _positionThumb;
        private readonly Thumb _topLeftThumb;
        private readonly Thumb _topRightThumb;
        private readonly Thumb _bottomLeftThumb;
        private readonly Thumb _bottomRightThumb;
        private readonly Thumb _topThumb;
        private readonly Thumb _leftThumb;
        private readonly Thumb _bottomThumb;
        private readonly Thumb _rightThumb;

        #endregion

        #region Constructor

        public CroppingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _visualCollection = new VisualCollection(this);

            var canvas = new Canvas();
            _visualCollection.Add(canvas);

            canvas.Children.Add(_positionThumb = BuildPositionThumb());
            canvas.Children.Add(_topLeftThumb = BuildCornerThumb(Cursors.SizeNWSE));
            canvas.Children.Add(_topRightThumb = BuildCornerThumb(Cursors.SizeNESW));
            canvas.Children.Add(_bottomLeftThumb = BuildCornerThumb(Cursors.SizeNESW));
            canvas.Children.Add(_bottomRightThumb = BuildCornerThumb(Cursors.SizeNWSE));
            canvas.Children.Add(_topThumb = BuildCornerThumb(Cursors.SizeNS));
            canvas.Children.Add(_leftThumb = BuildCornerThumb(Cursors.SizeWE));
            canvas.Children.Add(_bottomThumb = BuildCornerThumb(Cursors.SizeNS));
            canvas.Children.Add(_rightThumb = BuildCornerThumb(Cursors.SizeWE));

            _positionThumb.DragDelta += (s, a) => Move(a.HorizontalChange, a.VerticalChange);
            _topLeftThumb.DragDelta += (s, a) => Resize(SideFlags.TopLeft, a.HorizontalChange, a.VerticalChange);
            _topRightThumb.DragDelta += (s, a) => Resize(SideFlags.TopRight, a.HorizontalChange, a.VerticalChange);
            _bottomLeftThumb.DragDelta += (s, a) => Resize(SideFlags.BottomLeft, a.HorizontalChange, a.VerticalChange);
            _bottomRightThumb.DragDelta +=
                (s, a) => Resize(SideFlags.BottomRight, a.HorizontalChange, a.VerticalChange);
            _topThumb.DragDelta += (s, a) => Resize(SideFlags.Top, a.HorizontalChange, a.VerticalChange);
            _leftThumb.DragDelta += (s, a) => Resize(SideFlags.Left, a.HorizontalChange, a.VerticalChange);
            _bottomThumb.DragDelta += (s, a) => Resize(SideFlags.Bottom, a.HorizontalChange, a.VerticalChange);
            _rightThumb.DragDelta += (s, a) => Resize(SideFlags.Right, a.HorizontalChange, a.VerticalChange);

            if (!(adornedElement is FrameworkElement frameworkElement))
                return;

            var minimum = Math.Min(frameworkElement.ActualHeight, frameworkElement.ActualWidth);

            var x = (frameworkElement.ActualWidth - minimum) / 2d;

            var y = (frameworkElement.ActualHeight - minimum) / 2d;

            Selection = new Rect(new Point(x, y), new Size(minimum, minimum));

            frameworkElement.SizeChanged += AdornedElementSizeChanged;
        }

        #endregion

        #region Properties

        public double ScalingFactor
        {
            get
            {
                var image = AdornedElement as Image;

                if (!(image?.Source is BitmapImage bitmapImage))
                    return 1d;

                switch (image.Stretch)
                {
                    case Stretch.Uniform:
                        return Math.Max(
                            image.ActualWidth / bitmapImage.PixelWidth,
                            image.ActualHeight / bitmapImage.PixelHeight);

                    case Stretch.UniformToFill:
                        return Math.Min(
                            image.ActualWidth / bitmapImage.PixelWidth,
                            image.ActualHeight / bitmapImage.PixelHeight);
                }

                return 1d;
            }
        }

        #endregion

        #region Event Handlers

        private void AdornedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newPosition = Selection;

            newPosition.Scale(e.NewSize.Width / e.PreviousSize.Width, e.NewSize.Height / e.PreviousSize.Height);

            Selection = newPosition;
        }

        #endregion

        #region Overrides of UIElement

        protected override void OnRender(DrawingContext drawingContext)
        {
            Geometry maskGeometry = new CombinedGeometry(GeometryCombineMode.Exclude,
                new RectangleGeometry(new Rect(new Size(ActualWidth, ActualHeight))),
                new RectangleGeometry(Selection));

            drawingContext.DrawGeometry(Mask, null, maskGeometry);
        }

        #endregion

        #region Dependancy Properties

        #region Selection

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register("Selection", typeof(Rect), typeof(CroppingAdorner),
                new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnSelectionChanged, CoerceSelection));

        public Rect Selection
        {
            get => (Rect)GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }

        private static object CoerceSelection(DependencyObject d, object value)
        {
            var self = (CroppingAdorner)d;

            if (!(self.AdornedElement is FrameworkElement adornedElement))
                return value;

            var proposed = (Rect)value;

            var topLeft = new Point(Math.Max(proposed.Left, 0d), Math.Max(proposed.Top, 0d));

            var bottomRight = new Point(Math.Min(proposed.Right, adornedElement.ActualWidth),
                Math.Min(proposed.Bottom, adornedElement.ActualHeight));

            return new Rect(topLeft, bottomRight);
        }

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var self = (CroppingAdorner)d;

            self.UpdateThumbPositions();

            var e = new RoutedPropertyChangedEventArgs<Rect>(
                (Rect)args.OldValue, (Rect)args.NewValue, SelectionChangedEvent);

            self.OnSelectionChanged(e);
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Rect>), typeof(CroppingAdorner));

        public event RoutedPropertyChangedEventHandler<Rect> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        protected virtual void OnSelectionChanged(RoutedPropertyChangedEventArgs<Rect> args)
        {
            RaiseEvent(args);
        }

        #endregion

        #region Mask

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(Brush), typeof(CroppingAdorner),
                new PropertyMetadata(DefaultMask));

        public Brush Mask
        {
            get => (Brush)GetValue(MaskProperty);
            set => SetValue(MaskProperty, value);
        }

        #endregion

        #region ForceSquare

        public static readonly DependencyProperty ForceSquareProperty =
            DependencyProperty.Register("ForceSquare", typeof(bool), typeof(CroppingAdorner),
                new PropertyMetadata(false));

        public bool ForceSquare
        {
            get => (bool)GetValue(ForceSquareProperty);
            set => SetValue(ForceSquareProperty, value);
        }

        #endregion

        #region CroppingUnitType, MinimumSelectionHeight, MinimumSelectionWidth

        public static readonly DependencyProperty CroppingUnitTypeProperty =
            DependencyProperty.Register("CroppingUnitType", typeof(CroppingUnitType), typeof(CroppingAdorner),
                new PropertyMetadata(default(CroppingUnitType)));

        public CroppingUnitType CroppingUnitType
        {
            get => (CroppingUnitType)GetValue(CroppingUnitTypeProperty);
            set => SetValue(CroppingUnitTypeProperty, value);
        }

        public static readonly DependencyProperty MinimumSelectionHeightProperty =
            DependencyProperty.Register("MinimumSelectionHeight", typeof(double), typeof(CroppingAdorner),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None, null, CoerceMinimum));

        public double MinimumSelectionHeight
        {
            get => (double)GetValue(MinimumSelectionHeightProperty);
            set => SetValue(MinimumSelectionHeightProperty, value);
        }

        public static readonly DependencyProperty MinimumSelectionWidthProperty =
            DependencyProperty.Register("MinimumSelectionWidth", typeof(double), typeof(CroppingAdorner),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None, null, CoerceMinimum));

        public double MinimumSelectionWidth
        {
            get => (double)GetValue(MinimumSelectionWidthProperty);
            set => SetValue(MinimumSelectionWidthProperty, value);
        }

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            var value = (double)basevalue;

            if (double.IsInfinity(value) || double.IsNaN(value) || value < 0d)
                return 0d;

            return value;
        }

        #endregion

        #endregion

        #region Public Methods

        public Int32Rect GetPixelSelection()
        {
            var maxHeight = int.MaxValue;
            var maxWidth = int.MaxValue;

            if (AdornedElement is Image image)
            {
                if (image.Source is BitmapSource bitmapSource)
                {
                    maxHeight = bitmapSource.PixelHeight;
                    maxWidth = bitmapSource.PixelWidth;
                }
                else if (image.Source != null)
                {
                    maxHeight = (int)image.Source.Height;
                    maxWidth = (int)image.Source.Width;
                }
            }

            var scalingFactor = 1.0 / ScalingFactor;

            var left = Limit((int)Math.Round(Selection.Left * scalingFactor), 0, maxWidth);
            var top = Limit((int) Math.Round(Selection.Top * scalingFactor), 0, maxHeight);
            var width = Limit((int)Math.Round(Selection.Width * scalingFactor), 0, maxWidth - left);
            var height = Limit((int)Math.Round(Selection.Height * scalingFactor), 0, maxHeight - top);

            return new Int32Rect(left, top, width, height);
        }

        public BitmapSource GetCroppedBitmapSource()
        {
            if (!(AdornedElement is Image image))
                throw new InvalidOperationException(
                    "AdornedElement must be an Image control displaying a BitmapSource");

            if (!(image.Source is BitmapSource bitmapSource))
                throw new InvalidOperationException(
                    "AdornedElement must be an Image control displaying a BitmapSource");

            return new CroppedBitmap(bitmapSource, GetPixelSelection());
        }

        public BitmapSource GetCroppedBitmapSource(int height, int width)
        {
            if (height <= 0)
                throw new ArgumentException(@"height must be greater than 0", nameof(height));

            if (width <= 0)
                throw new ArgumentException(@"width must be greater than 0", nameof(width));

            var croppedBitmapSource = GetCroppedBitmapSource();

            if (croppedBitmapSource.PixelHeight <= 0 || croppedBitmapSource.PixelHeight <= 0)
                throw new InvalidOperationException("Selection has no area.");

            Transform transform = new ScaleTransform(
                (double)width / croppedBitmapSource.PixelHeight,
                (double)height / croppedBitmapSource.PixelHeight);

            return new TransformedBitmap(croppedBitmapSource, transform);
        }

        #endregion

        #region Private Methods

        private static Thumb BuildPositionThumb()
        {
            return new InvisibleThumb
            {
                Cursor = Cursors.SizeAll
            };
        }

        private static Thumb BuildCornerThumb(Cursor cursor)
        {
            return new Thumb
            {
                IsHitTestVisible = true,
                Height = ThumbSize,
                Width = ThumbSize,
                Cursor = cursor
            };
        }

        private static void SetPosition(FrameworkElement element, Point position)
        {
            SetPosition(element, position.X, position.Y);
        }

        private static void SetPosition(FrameworkElement element, double x, double y)
        {
            Canvas.SetLeft(element, x - element.ActualWidth / 2);
            Canvas.SetTop(element, y - element.ActualHeight / 2);
        }

        private void UpdateThumbPositions()
        {
            var centerX = Selection.Left + Selection.Width / 2d;
            var centerY = Selection.Top + Selection.Height / 2d;

            SetPosition(_topLeftThumb, Selection.TopLeft);
            SetPosition(_topRightThumb, Selection.TopRight);
            SetPosition(_bottomLeftThumb, Selection.BottomLeft);
            SetPosition(_bottomRightThumb, Selection.BottomRight);
            SetPosition(_topThumb, centerX, Selection.Top);
            SetPosition(_leftThumb, Selection.Left, centerY);
            SetPosition(_bottomThumb, centerX, Selection.Bottom);
            SetPosition(_rightThumb, Selection.Right, centerY);

            Canvas.SetLeft(_positionThumb, Selection.Left);
            Canvas.SetTop(_positionThumb, Selection.Top);
            _positionThumb.Height = Selection.Height;
            _positionThumb.Width = Selection.Width;
        }

        private static int Limit(int value, int minimum, int maximum)
        {
            if (value < minimum)
                return minimum;

            return value > maximum ? maximum : value;
        }

        private static double Limit(double value, double minimum, double maximum)
        {
            if (value < minimum)
                return minimum;

            if (value > maximum)
                return maximum;

            return value;
        }

        private static double RootMeanSquare(double a, double b)
        {
            return Math.Sqrt((a * a + b * b) / 2.0);
        }

        private double Scale(double value)
        {
            switch (CroppingUnitType)
            {
                case CroppingUnitType.BitmapPixels:
                    return value * ScalingFactor;

                case CroppingUnitType.DeviceIndependentPixels:
                    throw new NotImplementedException();

                default:
                    return value;
            }
        }

        private void Move(double horizontalChange, double verticalChange)
        {
            if (!(AdornedElement is FrameworkElement adornedElement))
                return;

            if (double.IsInfinity(horizontalChange) || double.IsNaN(horizontalChange))
                horizontalChange = 0d;

            if (double.IsInfinity(verticalChange) || double.IsNaN(verticalChange))
                verticalChange = 0d;

            if (Math.Abs(horizontalChange) < double.Epsilon && Math.Abs(verticalChange) < double.Epsilon)
                return;

            var location = new Point(
                Limit(Selection.Left + horizontalChange, 0d, adornedElement.ActualWidth - Selection.Width),
                Limit(Selection.Top + verticalChange, 0d, adornedElement.ActualHeight - Selection.Height));

            Selection = new Rect(location, Selection.Size);
        }

        private void Resize(SideFlags corner, double deltaX, double deltaY)
        {
            if (!(AdornedElement is FrameworkElement adornedElement))
                return;

            if (double.IsInfinity(deltaX) || double.IsNaN(deltaX))
                deltaX = 0d;

            if (double.IsInfinity(deltaY) || double.IsNaN(deltaY))
                deltaY = 0d;

            if ((corner & SideFlags.All) == SideFlags.None)
                return;

            if (Math.Abs(deltaX) < double.Epsilon && Math.Abs(deltaY) < double.Epsilon)
                return;

            // The fixed point of the selection that isn't moving
            var referencePoint = new Point();

            // The vector from the fixed point to the opposite corner of the selection
            var targetVector = new Vector();

            // Maximum length of either dimention of the selection, when forcing the selection to be square
            double maxSquare, signX, signY;

            if (corner.HasFlag(SideFlags.Left))
            {
                referencePoint.X = Selection.Right;

                var left = Limit(Selection.Left + deltaX, 0d, Selection.Right - Scale(MinimumSelectionWidth));
                targetVector.X = left - referencePoint.X;
                maxSquare = Selection.Right;

                signX = -1d;
            }
            else if (corner.HasFlag(SideFlags.Right))
            {
                referencePoint.X = Selection.Left;

                var right = Limit(Selection.Right + deltaX, Selection.Left + Scale(MinimumSelectionWidth),
                    adornedElement.ActualWidth);

                targetVector.X = right - referencePoint.X;
                maxSquare = adornedElement.ActualWidth - Selection.Left;

                signX = 1d;
            }
            else
            {
                referencePoint.X = Selection.Left;
                targetVector.X = Selection.Width;
                signX = 1d;

                maxSquare = double.PositiveInfinity;
            }

            if (corner.HasFlag(SideFlags.Top))
            {
                referencePoint.Y = Selection.Bottom;

                var top = Limit(Selection.Top + deltaY, 0d, Selection.Bottom - Scale(MinimumSelectionHeight));
                targetVector.Y = top - referencePoint.Y;
                maxSquare = Math.Min(maxSquare, Selection.Bottom);

                signY = -1d;
            }
            else if (corner.HasFlag(SideFlags.Bottom))
            {
                referencePoint.Y = Selection.Top;

                var bottom = Limit(Selection.Bottom + deltaY, Selection.Top + Scale(MinimumSelectionHeight),
                    adornedElement.ActualHeight);

                targetVector.Y = bottom - referencePoint.Y;
                maxSquare = Math.Min(maxSquare, adornedElement.ActualHeight - Selection.Top);

                signY = 1d;
            }
            else
            {
                referencePoint.Y = Selection.Top;
                targetVector.Y = Selection.Height;
                signY = 1d;
            }

            if (ForceSquare)
            {
                var sideLength = Limit(RootMeanSquare(targetVector.X, targetVector.Y), 0d, maxSquare);
                targetVector = new Vector(sideLength * signX, sideLength * signY);
            }

            Selection = new Rect(referencePoint, referencePoint + targetVector);
        }

        #endregion


        #region Overrides of FrameworkElement

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateThumbPositions();

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (UIElement child in _visualCollection)
                child.Arrange(new Rect(finalSize));

            return finalSize;
        }

        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        #endregion

        #region Private Enum

        [Flags]
        private enum SideFlags
        {
            None = 0x0,
            Top = 0x1,
            Left = 0x2,
            Bottom = 0x4,
            Right = 0x8,

            TopLeft = Top | Left,

            TopRight = Top | Right,

            BottomLeft = Bottom | Left,

            BottomRight = Bottom | Right,

            All = Top | Left | Bottom | Right
        }

        #endregion

        #region Private Classes

        private class InvisibleThumb : Thumb
        {
            protected override void OnRender(DrawingContext drawingContext)
            {
                var rect = new Rect(new Size(ActualHeight, ActualWidth));

                drawingContext.DrawRectangle(Brushes.Transparent, null, rect);
            }

            protected override Visual GetVisualChild(int index)
            {
                return null;
            }
        }

        #endregion
    }
}
