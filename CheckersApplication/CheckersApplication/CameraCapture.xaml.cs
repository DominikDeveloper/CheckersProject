using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Interop;

namespace CheckersApplication
{
    /// <summary>
    /// Interaction logic for CameraCapture.xaml
    /// </summary>
    public partial class CameraCapture : Page
    {
        ImageViewer cameraWindow;
        Capture capture;

        public CameraCapture()
        {
            InitializeComponent();

            cameraWindow = new ImageViewer(); //create an image viewer
            
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                cameraWindow.Image = capture.QueryFrame(); //.QuerySmallFrame(); --> what better?
                IMG_Camera.Source = ToBitmapSource(cameraWindow.Image);
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        }

        public void CameraShow()
        {
            ComponentDispatcher.ThreadIdle += new System.EventHandler(updateFrames);
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            BT_Start.IsEnabled = false;
            BT_Stop.IsEnabled = true;


            //capture:
            //0 parameters --> default camera; for DroidCam (Android phone)? --> capture = new Capture("http://IP:PORT/mjpegfeed");
            if (TB_CameraSource.Text != String.Empty)
            {
                Int32 idCamera;
                bool textParamIsNumber = Int32.TryParse(TB_CameraSource.Text, out idCamera);

                if (textParamIsNumber == true)
                    capture = new Capture(idCamera); //number in textbox = idCamera as parameter
                else
                    capture = new Capture(TB_CameraSource.Text); //string in textbox = url
            }
            else
            {
                capture = new Capture(); //blank textbox = default camera
            }
            
            CameraShow();
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            BT_Stop.IsEnabled = false;
            BT_Start.IsEnabled = true;

            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            capture.Stop();
            capture.Dispose();
        }

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            try
            {
                using (Bitmap source = image.Bitmap)
                {
                    IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                    BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ptr,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    DeleteObject(ptr); //release the HBitmap
                    return bs;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
    }
}
