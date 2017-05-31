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
        public static  System.Windows.Media.Color player1Color = new System.Windows.Media.Color();
        public static  System.Windows.Media.Color player2Color = new System.Windows.Media.Color();
        public static bool player1Detected = false;
        public static bool player2Detected = false;

        public MainWindow()
        {
            InitializeComponent();
            DiscoverUsbCameras();
            InitValuePickers();
            CvInvoke.UseOpenCL = (bool)CB_OpenCL.IsChecked;
            this.ChessBoard.ItemsSource = chessBoardState.pieces;
        }

        private void InitValuePickers() //set values for white
        {
            blueSlider1.Value = blueSlider1.Maximum;
            blueSlider2.Value = blueSlider2.Maximum;
            blueSlider3.Value = blueSlider3.Maximum;
            blueSlider4.Value = blueSlider4.Maximum;

            greenSlider1.Value = greenSlider1.Maximum;
            greenSlider2.Value = greenSlider2.Maximum;
            greenSlider3.Value = greenSlider3.Maximum;
            greenSlider4.Value = greenSlider4.Maximum;

            redSlider1.Value = redSlider1.Maximum;
            redSlider2.Value = redSlider2.Maximum;
            redSlider3.Value = redSlider3.Maximum;
            redSlider4.Value = redSlider4.Maximum;
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
                var image = new Image<Bgr, Byte>(camera.imageViewer.Image.Bitmap);
                Detect(cameraCapture: image);
            }
            catch (Exception ex)
            {
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
            if (CB_DefaultCamera.IsChecked == true && CO_Cameras.Items.Count > 0)
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
            List<Color> currentColors = new List<Color>();


            if (filePath != null)
                image = new Image<Bgr, byte>(filePath).Resize(400, 400, Inter.Linear, true);
            else
                image = cameraCapture;

            resultImage = image.Copy();
            System.Drawing.Point[] points = Detection.GetRectanglePoints(image);

            if (points != null)
            {
                resultImage.Draw(points, new Bgr(Color.DarkOrange), 2);
                ChessField[,] fields = ChessField.GetChessFields(points);
                if (fields != null)
                {
                    foreach (var field in fields)
                        resultImage.Draw(field.points, new Bgr(Color.Green), 2);

                    CircleF[] circles = Detection.GetCircles(image);


                    Mat filtring1 = new Mat();
                    Mat filtring2 = new Mat();

                    int[] minColorVal = new int[] { 3, 2, 1 };

                    if (circles != null)
                    {
                        if (CB_AutoDetectColors.IsChecked == true)
                        {
                            foreach (CircleF circle in circles)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Red), 3);
                                ChessField.Pons(fields, circles);
                                chessBoardState.Clear();
                                chessBoardState.AddPieces(fields);
                            }

                            Detection.DetectPlayersColors(circles, ref player1Color, ref player2Color, image);

                            CV_Player1Color.Background = new System.Windows.Media.SolidColorBrush(player1Color);
                            redSlider1.Value = player1Color.R;
                            greenSlider1.Value = player1Color.G;
                            blueSlider1.Value = player1Color.B;
                            CV_Player2Color.Background = new System.Windows.Media.SolidColorBrush(player2Color);
                            redSlider2.Value = player2Color.R;
                            greenSlider2.Value = player2Color.G;
                            blueSlider2.Value = player2Color.B;
                        }
                        else
                        {
                            var circleFor1 = Detection.FilterSomeColors(
                                image,
                                ref filtring1,
                                new double[] { blueSlider1.Value, greenSlider1.Value, redSlider1.Value },
                                new double[] { blueSlider3.Value, greenSlider3.Value, redSlider3.Value });
                            var circleFor2 = Detection.FilterSomeColors(
                                image,
                                ref filtring2,
                                new double[] { blueSlider2.Value, greenSlider2.Value, redSlider2.Value },
                                new double[] { blueSlider4.Value, greenSlider4.Value, redSlider4.Value });

                            chessBoardState.Clear();

                            foreach (CircleF circle in circleFor1)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Green), 3);
                                ChessField.Pons(fields, circles);
                                chessBoardState.AddPieces(fields);
                            }

                            foreach (CircleF circle in circleFor2)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Blue), 3);
                                ChessField.Pons(fields, circles);
                                chessBoardState.AddPieces(fields);
                            }

                            IMG_Filter1.Source = ToBitmapConverter.Convert(filtring1);
                            IMG_Filter2.Source = ToBitmapConverter.Convert(filtring2);
                        }
                        
                    }
                }
            }
            IMG_Detected.Source = ToBitmapConverter.Convert(resultImage);
        }

        private void RGBplayer1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb((byte)redSlider1.Value, (byte)greenSlider1.Value, (byte)blueSlider1.Value);
            CV_Player1Color.Background = new System.Windows.Media.SolidColorBrush(color);
        }

        private void RGBplayer2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb((byte)redSlider2.Value, (byte)greenSlider2.Value, (byte)blueSlider2.Value);
            CV_Player2Color.Background = new System.Windows.Media.SolidColorBrush(color);
        }

        private void RGBplayer3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb((byte)redSlider3.Value, (byte)greenSlider3.Value, (byte)blueSlider3.Value);
            CV_Player1Color_Max.Background = new System.Windows.Media.SolidColorBrush(color);
        }

        private void RGBplayer4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb((byte)redSlider4.Value, (byte)greenSlider4.Value, (byte)blueSlider4.Value);
            CV_Player2Color_Max.Background = new System.Windows.Media.SolidColorBrush(color);
        }

        private void CB_AutoDetectColors_Click(object sender, RoutedEventArgs e)
        {
            if (CB_AutoDetectColors.IsChecked == true)
            {
                redSlider1.IsEnabled = false;
                redSlider2.IsEnabled = false;
                blueSlider1.IsEnabled = false;
                blueSlider2.IsEnabled = false;
                greenSlider1.IsEnabled = false;
                greenSlider2.IsEnabled = false;
                GD_MaxSettingColors.Visibility = Visibility.Hidden;
                GD_FilterImages.Visibility = Visibility.Hidden;
            }
            else
            {
                redSlider1.IsEnabled = true;
                redSlider2.IsEnabled = true;
                blueSlider1.IsEnabled = true;
                blueSlider2.IsEnabled = true;
                greenSlider1.IsEnabled = true;
                greenSlider2.IsEnabled = true;
                GD_MaxSettingColors.Visibility = Visibility.Visible;
                GD_FilterImages.Visibility = Visibility.Visible;
            }
        }
    }

}
