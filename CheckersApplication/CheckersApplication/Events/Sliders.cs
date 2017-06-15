using System.Windows;

namespace CheckersApplication
{
    public partial class MainWindow : Window
    {
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

        private void RangeSlider_Player1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (ifInitComponent)
            {
                LB_Range1R.Content = RS_Slider1R.LowerValue.ToString() + " - " + RS_Slider1R.HigherValue.ToString();
                LB_Range1G.Content = RS_Slider1G.LowerValue.ToString() + " - " + RS_Slider1G.HigherValue.ToString();
                LB_Range1B.Content = RS_Slider1B.LowerValue.ToString() + " - " + RS_Slider1B.HigherValue.ToString();

                if (CB_AutoDetectColors.IsChecked == false)
                {
                    System.Windows.Media.Color minColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.LowerValue, (byte)RS_Slider1G.LowerValue, (byte)RS_Slider1B.LowerValue);
                    CV_Player1Color_Min.Background = new System.Windows.Media.SolidColorBrush(minColor);

                    System.Windows.Media.Color maxColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.HigherValue, (byte)RS_Slider1G.HigherValue, (byte)RS_Slider1B.HigherValue);
                    CV_Player1Color_Max.Background = new System.Windows.Media.SolidColorBrush(maxColor);

                    System.Windows.Application.Current.Resources["Player1Color"] = CV_Player1Color_Max.Background;
                }
            }
        }

        private void RangeSlider_Player2_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (ifInitComponent)
            {
                LB_Range2R.Content = RS_Slider2R.LowerValue.ToString() + " - " + RS_Slider2R.HigherValue.ToString();
                LB_Range2G.Content = RS_Slider2G.LowerValue.ToString() + " - " + RS_Slider2G.HigherValue.ToString();
                LB_Range2B.Content = RS_Slider2B.LowerValue.ToString() + " - " + RS_Slider2B.HigherValue.ToString();

                if (CB_AutoDetectColors.IsChecked == false)
                {
                    System.Windows.Media.Color minColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.LowerValue, (byte)RS_Slider2G.LowerValue, (byte)RS_Slider2B.LowerValue);
                    CV_Player2Color_Min.Background = new System.Windows.Media.SolidColorBrush(minColor);

                    System.Windows.Media.Color maxColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.HigherValue, (byte)RS_Slider2G.HigherValue, (byte)RS_Slider2B.HigherValue);
                    CV_Player2Color_Max.Background = new System.Windows.Media.SolidColorBrush(maxColor);

                    System.Windows.Application.Current.Resources["Player2Color"] = CV_Player2Color_Max.Background;
                }
            }
        }
    }
}
