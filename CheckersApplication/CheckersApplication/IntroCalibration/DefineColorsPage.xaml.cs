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
    /// Interaction logic for DefineColorsPage.xaml
    /// </summary>
    public partial class DefineColorsPage : Page
    {
        Camera camera;

        public DefineColorsPage()
        {
            InitializeComponent();
        }

        public DefineColorsPage(Camera cam)
        {
            InitializeComponent();
            this.camera = cam;
            ComponentDispatcher.ThreadIdle += new System.EventHandler(updateFrames);
        }

        private void BTN_Next_Click(object sender, RoutedEventArgs e)
        {
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            NavigationService.Navigate(new DefineColors2Page(camera));
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame();

                string[] minHSV = new string[3] { IUD_MinH.Text, IUD_MinS.Text, IUD_MinV.Text };

                string[] maxHSV = new string[3] { IUD_MaxH.Text, IUD_MaxS.Text, IUD_MaxV.Text };

                var color_hue_image = Detection.DrawDetectedColorCircle(
                    camera.imageViewer.Image.Bitmap, minHSV, maxHSV, ref HSV_PiecesColors.Player1_MCvS_Data);

                IMG_Camera.Source = ToBitmapConverter.Convert(color_hue_image);

                IMG_ShowMinColor.Source = PreviewColor(HSV_PiecesColors.Player1_MCvS_Data[(int)ColorValArray.Minimum]);
                IMG_ShowMaxColor.Source = PreviewColor(HSV_PiecesColors.Player1_MCvS_Data[(int)ColorValArray.Maximum]);
            }
            catch (Exception ex)
            {
                ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private BitmapSource PreviewColor(MCvScalar color, int width = 60, int height = 60)
        {
            Image<Hsv, byte> image = new Image<Hsv, byte>(width, height);
            CvInvoke.Rectangle(image, new System.Drawing.Rectangle(0, 0, width, height), color, -1);

            return ToBitmapConverter.Convert(image);
        }
    }
}
