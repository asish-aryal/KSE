using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Kinect;

namespace Kinect_Explorer
{
    /// <summary>
    /// Interaction logic for TempWindow.xaml
    /// </summary>
    public partial class ViewerWindow : Window
    {
		

        ViewManager vManager;



        public ViewerWindow()
        {
            InitializeComponent();

        }




        private void Window_Closing(object sender, EventArgs e)
        {
            vManager.stopKinect();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vManager = ViewManager.getInstance();
            vManager.start();
            
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //repaint();
        }


        private void KeyboardInput(object sender, KeyEventArgs e)
        {
            vManager.keyboard_input(sender, e);
        }
    }
}
