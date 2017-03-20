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
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

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
            capture = new Capture(); //create a camera capture
            //0 parameters --> default camera; for DroidCam (Android phone)? --> capture = new Capture("http://IP:PORT/mjpegfeed");
            CameraShow();
        }

        public void updateFrames(object sender, EventArgs e)
        {
            cameraWindow.Image = capture.QueryFrame();
        }

        public void CameraShow()
        {
            System.Windows.Forms.Application.Idle += new EventHandler(updateFrames);
            cameraWindow.ShowDialog();         
        }
    }
}
