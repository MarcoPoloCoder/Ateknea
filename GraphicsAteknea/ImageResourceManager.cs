using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicsAteknea
{
    public class ImageResourceManager
    {
        private ImageSourceConverter _converter;

        public ImageResourceManager()
        {
            _converter = new ImageSourceConverter();
        }

        public ImageSource GetStatusLedsResource(bool stat)
        {
            if (stat)
            {
                return GetImageStream(GraphicsAteknea.Properties.Resources.LedRed);
            }
            else
            {
                return GetImageStream(GraphicsAteknea.Properties.Resources.LedGreen);
            }
        }

        public static BitmapSource GetImageStream(Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

    }
    [ValueConversion(typeof(Bitmap), typeof(ImageSource))]
    public class BitmapToImageSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bmp = value as Bitmap;
            if (bmp == null)
                return null;
            using (Stream str = new MemoryStream())
            {
                bmp.Save(str, System.Drawing.Imaging.ImageFormat.Bmp);
                str.Seek(0, SeekOrigin.Begin);
                BitmapDecoder bdc = new BmpBitmapDecoder(str, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return bdc.Frames[0];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
