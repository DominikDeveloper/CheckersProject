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
