using System.Drawing;
using System.Windows;

namespace Picosa.Drawing
{
    public static class Extensions
    {
        public static Rectangle ToRectangle(this Rect rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}
