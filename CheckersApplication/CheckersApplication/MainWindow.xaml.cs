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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Collections.ObjectModel;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;

using DirectShowLib;

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

            discoverUsbCameras();

            CvInvoke.UseOpenCL = (bool)CB_OpenCL.IsChecked;
        }

        public void discoverUsbCameras()
        {
            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();

            //-> Find systems cameras with DirectShow.Net dll
            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            int _DeviceIndex = 0;
            foreach (DirectShowLib.DsDevice _Camera in _SystemCamereas)
            {
                CO_Cameras.Items.Add(new KeyValuePair<int, string>(_DeviceIndex, _Camera.Name));
                _DeviceIndex++;
            }

            if (CO_Cameras.Items.Count > 0)
                CO_Cameras.SelectedIndex = 0;
        }

            InitializeComponent();
            FillChessboard();
        }

        private void FillChessboard()
        {
            ChessBoardState chessBoardState = new ChessBoardState();
            //chessBoardState.TestData();
            this.ChessBoard.ItemsSource = chessBoardState.pieces;
        }
            
        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame(); //.QuerySmallFrame(); --> what better?
                camera.imageViewer.Image = Test2OnCamera(camera.imageViewer.Image, 8, 8); //after testing, delete it
                IMG_Camera.Source = ToBitmapConverter.Convert(camera.imageViewer.Image);              
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
                if (CO_Cameras.Items.Count > 0)
                    camera = new Camera(Convert.ToString(CO_Cameras.SelectedIndex));
            }
            else
            {
                camera = new Camera(TB_CameraSource.Text);
            }
            CameraShow();
            CO_Cameras.IsEnabled = false;
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            ChangeBtnStartStop();
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            camera.capture.Stop();
            camera.capture.Dispose();
            IMG_Camera.Source = null;

            CO_Cameras.IsEnabled = true;
        }

        private void CB_DefaultCamera_Click(object sender, RoutedEventArgs e)
        {
            CO_Cameras.IsEnabled = !CO_Cameras.IsEnabled;

            TB_CameraSource.IsEnabled = !(bool)CB_DefaultCamera.IsChecked;
            if (!TB_CameraSource.IsEnabled)
            {
                TB_CameraSource.Text = "URL streamu, ID kamery, lub plik wideo,\nnp. (http://IP:PORT/mjpegfeed)";
                TB_CameraSource.Foreground = System.Windows.Media.Brushes.Gray;
            }
            
        }

        private void CB_OpenCL_Click(object sender, RoutedEventArgs e)
        {
            CvInvoke.UseOpenCL = !CvInvoke.UseOpenCL;
        }

        private void BT_ImageTest_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;


            DialogResult result = openFileDialog1.ShowDialog();

            string path = String.Empty;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    path = System.IO.Path.GetFileName(openFileDialog1.FileName);
                    if (path != String.Empty)
                        Test1OnPicture(path);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Niepoprawny plik. Komunikat błędu: " + ex.Message);
                }
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

            var imgToCorners = new Image<Bgr, Byte>(filePath);
            CvInvoke.Imshow("Result of corners browsing", detection.GetInternalCorners(imgToCorners, width, height));

            var imgToRectangles = new Image<Bgr, byte>(filePath).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            CvInvoke.Imshow("Result of triangles and rectangles browsing", detection.GetTrianglesRectangles(imgToRectangles));

            var imgToCircles =
                new Image<Bgr, byte>(filePath).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            CvInvoke.Imshow("Result of circles browsing", detection.GetCircles(imgToCircles));
        }


        public IImage Test2OnCamera(IImage img, UInt16 width, UInt16 height)
        {
            #region corners
            var detCorners = detection.GetInternalCorners(img, width, height);
            //CvInvoke.Imshow("Corners-Circles-Rects", detCorners);
            #endregion

            #region 3/4 -angles
            Bitmap bmp = new Bitmap(img.Bitmap);
            var convertedImg = new Image<Bgr, Byte>(bmp);

            var detRects = detection.GetTrianglesRectangles(convertedImg, false);
            //CvInvoke.Imshow("Corners-Circles-Rects", detRects);
            #endregion

            #region circles
            bmp = new Bitmap(detRects.Bitmap);
            convertedImg = new Image<Bgr, Byte>(bmp);

            var detCircles = detection.GetCircles(convertedImg);
            //CvInvoke.Imshow("Corners-Circles-Rects", detCircles);
            #endregion

            return detCircles; //last modified picture
        }

    }
}
