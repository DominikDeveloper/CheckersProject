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
        ChessBoardState chessBoardState = new ChessBoardState();

        public MainWindow()
        {
            InitializeComponent();            
            DiscoverUsbCameras();
            CvInvoke.UseOpenCL = (bool)CB_OpenCL.IsChecked;
            this.ChessBoard.ItemsSource = chessBoardState.pieces;
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
                
        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame();
                IMG_Camera.Source = ToBitmapConverter.Convert(camera.imageViewer.Image);
                var image = new Image<Bgr, Byte>(camera.imageViewer.Image.Bitmap);
                Detect(cameraCapture: image);
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
                    Detect(openFileDialog1.FileName);
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

        private void Detect(string filePath = null, Image<Bgr, byte> cameraCapture = null)
        {
            Image<Bgr, byte> image, resultImage;


            if (filePath != null)
                image = new Image<Bgr, byte>(filePath).Resize(400, 400, Inter.Linear, true);
            else
                image = cameraCapture;

            resultImage = image.Copy();
            System.Drawing.Point[]points = Detection.GetRectanglePoints(image);

            if (points != null)
            {
                resultImage.Draw(points, new Bgr(Color.DarkOrange), 2);
                ChessField[,] fields = ChessField.GetChessFields(points);
                if (fields != null)
                {
                    foreach (var field in fields)
                        resultImage.Draw(field.points, new Bgr(Color.Green), 2);

                    CircleF[] circles = Detection.GetCircles(image);

                    if (circles != null)
                    {
                        foreach (CircleF circle in circles)
                            resultImage.Draw(circle, new Bgr(Color.Blue), 3);

                        ChessField.Pons(fields, circles);
                        chessBoardState.Clear();
                        chessBoardState.AddPieces(fields);

                    }
                }
            }
            IMG_Detected.Source = ToBitmapConverter.Convert(resultImage);
        }

    }
}
