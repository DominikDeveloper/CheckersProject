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
using System.Collections.ObjectModel;

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
            this.ChessBoard.ItemsSource = new ObservableCollection<CheckersPiece>
        {
            new CheckersPiece{Pos=new System.Drawing.Point(0, 6), Player=Player.White},
            new CheckersPiece{Pos=new System.Drawing.Point(1, 6), Player=Player.White},
            new CheckersPiece{Pos=new System.Drawing.Point(2, 6), Player=Player.White},
            new CheckersPiece{Pos=new System.Drawing.Point(3, 6), Player=Player.Black},
            new CheckersPiece{Pos=new System.Drawing.Point(7, 0), Player=Player.Black}
        };

        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame(); //.QuerySmallFrame(); --> what better?
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

    }
}
