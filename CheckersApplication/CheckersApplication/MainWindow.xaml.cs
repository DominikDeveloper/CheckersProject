using System;
using System.Windows;
using System.Windows.Media.Imaging;
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
            DiscoverUsbCameras();
            CvInvoke.UseOpenCL = (bool)CB_OpenCL.IsChecked;
            FillChessboard();
        }

        public void DiscoverUsbCameras()
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

        private void FillChessboard()
        {
            ChessBoardState chessBoardState = new ChessBoardState();
            //chessBoardState.TestData();
            this.ChessBoard.ItemsSource = chessBoardState.pieces;
        }

        public void FillChessboard_tmp(int[,] ChessboardArray)
        {
            ChessBoardState chessBoardState = new ChessBoardState();
            chessBoardState.matrixToPieces_tmp(ChessboardArray);
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
            if (CB_DefaultCamera.IsChecked==true && CO_Cameras.Items.Count>0)     
                camera = new Camera(Convert.ToString(CO_Cameras.SelectedIndex));
            else
                camera = new Camera(TB_CameraSource.Text);
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
            openFileDialog1.Filter = "Image files (*.jpg, *.png) | *.jpg; *.png";
            openFileDialog1.RestoreDirectory = true;
            DialogResult result = openFileDialog1.ShowDialog();
            string path = String.Empty;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Test1OnPicture(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Komunikat błędu: " + ex.Message);
                }
            }
        }

        private void TB_CameraSource_GotFocus(object sender, RoutedEventArgs e)
        {
            TB_CameraSource.Text = "";
            TB_CameraSource.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void Test1OnPicture(string filePath)
        {
            UInt16 width = 8;
            UInt16 height = 8;

            var loadedImage = new Image<Bgr, Byte>(filePath).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
            ImageData testImage1 = new ImageData(loadedImage, width, height);

            //CORNERS
            var cornerPoints = testImage1.GetInternalCorners();
            var imgWithCorners = testImage1.GetDrawingCorners(cornerPoints);
            CvInvoke.Imshow("Result of corners browsing", imgWithCorners);

            //RECTANGLES
            List<RotatedRect> rectangles = testImage1.GetRectangles();
            var imgToRectangles = testImage1.GetDrawingRectangles(rectangles);
            CvInvoke.Imshow("Result of rectangles browsing", imgToRectangles);

            //CIRCLES
            CircleF[] circles = testImage1.GetCirclesPositions();
            var imgToCircles = testImage1.GetDrawingCircles(circles);
            CvInvoke.Imshow("Result of circles browsing", imgToCircles);


            int[,] cf =  detection.RepresentCircles(circles, rectangles);
            FillChessboard_tmp(cf);
        }

        public IImage Test2OnCamera(IImage img, UInt16 width, UInt16 height)
        {
            try
            {
                ImageData testImage1 = new ImageData(img, width, height);

                #region corners
                var cornersPoints = testImage1.GetInternalCorners();
                var imgWithCorners = testImage1.GetDrawingCorners(cornersPoints);
                #endregion

                #region 3/4 -angles
                ImageData testImage2 = new ImageData(imgWithCorners, width, height);

                var rectangles = testImage2.GetRectangles();
                var imgWithRects = testImage2.GetDrawingRectangles(rectangles);
                #endregion

                #region circles
                ImageData testImage3 = new ImageData(imgWithRects, width, height);

                ChessField[,] cf = new ChessField[8, 8];
                var circles = testImage3.GetRectangles();
                var imgWithCircles = testImage3.GetDrawingRectangles(rectangles);
                #endregion

                ImageData testImageResult = new ImageData(imgWithCircles, width, height);
                return testImageResult.GetSourceImage(); //last modified picture

            } catch (Exception excpt)
            {

            }
            
            return null;
        }

    }
}
