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
using System.Windows.Shapes;

namespace CheckersApplication
{
    /// <summary>
    /// Logika interakcji dla klasy CamConfigWindow.xaml
    /// </summary>
    public partial class CamConfigWindow : Window
    {
        public CamConfigWindow()
        {
            InitializeComponent();
            ModuleFrame.NavigationService.Navigate(new DefineColorsPage());
        }

        public CamConfigWindow(Camera cam)
        {
            InitializeComponent();
            ModuleFrame.NavigationService.Navigate(new DefineColorsPage(cam));
        }
    }
}
