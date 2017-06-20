using System;
using System.Windows;
using System.Windows.Forms;

using Emgu.CV;


namespace CheckersApplication
{
    public partial class MainWindow : Window
    {
        private void CB_OpenCL_Click(object sender, RoutedEventArgs e)
        {
            CvInvoke.UseOpenCL = !CvInvoke.UseOpenCL;
        }

        private void CB_AutoDetectColors_Click(object sender, RoutedEventArgs e)
        {
            if (CB_AutoDetectColors.IsChecked == true)
            {
                RS_Slider1R.IsEnabled = false;
                RS_Slider2R.IsEnabled = false;
                RS_Slider1G.IsEnabled = false;
                RS_Slider2G.IsEnabled = false;
                RS_Slider1B.IsEnabled = false;
                RS_Slider2B.IsEnabled = false;
            }
            else
            {
                RS_Slider1R.IsEnabled = true;
                RS_Slider2R.IsEnabled = true;
                RS_Slider1G.IsEnabled = true;
                RS_Slider2G.IsEnabled = true;
                RS_Slider1B.IsEnabled = true;
                RS_Slider2B.IsEnabled = true;
            }
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
                    fileName = openFileDialog1.FileName;
                    Detect(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Komunikat błędu: " + ex.Message);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Detect(fileName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Nie wczytano obrazka.");
            }
        }
    }
}
