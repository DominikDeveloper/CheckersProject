using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using System.Text;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace CheckersApplication
{
    /// <summary>
    /// Interaction logic for CameraCapture.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Camera camera;
        Detection detection;

        public MainWindow()
        {
            InitializeComponent();
            detection = new Detection();

            Test1OnPicture("Chessboard.png"); //after testing, delete it
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame(); //.QuerySmallFrame(); --> what better?
                //IMG_Camera.Source = ToBitmapConverter.Convert(camera.imageViewer.Image);
                Test2OnCamera(camera.imageViewer.Image, 8, 8); //after testing, delete it and uncomment @up
            }
            catch (Exception ex) {
                ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
                System.Windows.MessageBox.Show(ex.Message);
                ChangeBtnStartStop();
            }
        }

        private void ChangeBtnStartStop()
        {
            BT_Start.IsEnabled = !BT_Start.IsEnabled;
            BT_Stop.IsEnabled = !BT_Stop.IsEnabled;
        }

        public void CameraShow()
        {
            ComponentDispatcher.ThreadIdle += new System.EventHandler(updateFrames);
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            ChangeBtnStartStop();
            if (CB_DefaultCamera.IsChecked==true)
            {
                camera = new Camera("");
            }
            else
            {
                camera = new Camera(TB_CameraSource.Text);
            }
            CameraShow();           
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            ChangeBtnStartStop();
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            camera.capture.Stop();
            camera.capture.Dispose();
            IMG_Camera.Source = null;
        }

        private void CB_DefaultCamera_Click(object sender, RoutedEventArgs e)
        {
            TB_CameraSource.IsEnabled = !(bool)CB_DefaultCamera.IsChecked;
            if (!TB_CameraSource.IsEnabled)
            {
                TB_CameraSource.Text = "URL streamu, ID kamery, lub plik wideo,\nnp. (http://IP:PORT/mjpegfeed)";
                TB_CameraSource.Foreground = System.Windows.Media.Brushes.Gray;
            }
            
        }

        private void TB_CameraSource_GotFocus(object sender, RoutedEventArgs e)
        {
            TB_CameraSource.Text = "";
            TB_CameraSource.Foreground = System.Windows.Media.Brushes.Black;
        }

        /// <summary>
        /// Tests
        /// </summary>
        private void Test1OnPicture(string filePath)
        {
            UInt16 width = 8;
            UInt16 height = 8;
            Image<Gray, Byte> image = new Image<Gray, Byte>(filePath);
            detection.ShowCorners(image, width, height);
            detection.ShowCircles();
        }

        public void Test2OnCamera(IImage img, UInt16 width, UInt16 height)
        {
            detection.ShowCorners(img, 8, 8);
        }
    }
}
