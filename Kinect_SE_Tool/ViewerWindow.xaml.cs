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
		

        private ViewManager vManager;
        

        private List<Observer> observers = new List<Observer>();

        public ViewerWindow()
        {
            InitializeComponent();
            
        }

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer o in observers)
            {
                o.Update();
            }
        }







        private void Window_Closing(object sender, EventArgs e)
        {
            Notify();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vManager = ViewManager.getInstance();
            //vManager.start();
            
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Notify();
        }


        private void KeyboardInput(object sender, KeyEventArgs e)
        {
            //lastKey = e.Key;
            //Notify();
            
            vManager.keyboard_input(sender, e);
        }
    }
}
