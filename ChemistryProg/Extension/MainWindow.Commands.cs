using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChemistryProg
{
    namespace Commands
    {
        public class CustomCommands
        {
            public static RoutedCommand Exit = new RoutedCommand("Exit", typeof(CustomCommands));
            public static RoutedCommand TakeScreenshoot = new RoutedCommand("TakeScreenshoot", typeof(CustomCommands));
        }
    }

    partial class MainWindow
    {
        private const int ScreenShootDpi = 384;

        private void TakeScreenshoot_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                InitialDirectory = ""
            };
            if (saveFileDialog.ShowDialog() != true)
                return;

            RenderTargetBitmap renderTargetBitmap =
                     new RenderTargetBitmap((int)(monitor.ActualWidth * ScreenShootDpi / 96),
                     (int)(monitor.ActualHeight * ScreenShootDpi / 96), ScreenShootDpi, ScreenShootDpi, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(monitor);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create(saveFileDialog.FileName))
            {
                pngImage.Save(fileStream);
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
