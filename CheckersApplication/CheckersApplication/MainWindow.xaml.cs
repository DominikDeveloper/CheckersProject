using System;
using System.Windows;
using System.Windows.Media.Imaging;
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
    public partial class MainWindow : Window
    {
        Camera camera;

        public MainWindow()
        {
            InitializeComponent(); 
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame(); //.QuerySmallFrame(); --> what better?
                IMG_Camera.Source = ToBitmapConverter.Convert(camera.imageViewer.Image);
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
            camera = new Camera(TB_CameraSource.Text);
            CameraShow();           
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            BT_Stop.IsEnabled = false;
            BT_Start.IsEnabled = true;
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            camera.capture.Stop();
            camera.capture.Dispose();
            IMG_Camera.Source = null;
        }




    }
}
