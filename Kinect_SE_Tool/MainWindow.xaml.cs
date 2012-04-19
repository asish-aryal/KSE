using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinect_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void begin_Click(object sender, RoutedEventArgs e)
        {
            ViewerWindow window = ViewerWindow.getInstance();
            window.Show();
            this.Close();

        }

        private void test_window(object sender, EventArgs e)
        {
            TestWindow window = new TestWindow();
            window.Show();
            this.Close();
        }

        private void test_window2(object sender, MouseButtonEventArgs e)
        {
            TestWindow2 window = new TestWindow2();
            window.Show();
            this.Close();
        }
        

    }
}
