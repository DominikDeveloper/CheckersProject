using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckersApplication
{
    /// <summary>
    /// Interaction logic for DefineColors2Page.xaml
    /// </summary>
    public partial class DefineColors2Page : Page
    {
        Camera camera;

        public DefineColors2Page()
        {
            InitializeComponent();
        }

        public DefineColors2Page(Camera cam)
        {
            InitializeComponent();
            this.camera = cam;
            ComponentDispatcher.ThreadIdle += new System.EventHandler(updateFrames);
        }

        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);

            var window = Window.GetWindow(this);
            window.Close();
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame();

                string[] minHSV = new string[3] { IUD_MinH.Text, IUD_MinS.Text, IUD_MinV.Text };

                string[] maxHSV = new string[3] { IUD_MaxH.Text, IUD_MaxS.Text, IUD_MaxV.Text };

                var color_hue_image = Detection.DrawDetectedColorCircle(
                    camera.imageViewer.Image.Bitmap, minHSV, maxHSV, ref HSV_PiecesColors.Player2_MCvS_Data);

                IMG_Camera.Source = ToBitmapConverter.Convert(color_hue_image);
            }
            catch (Exception ex)
            {
                ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
