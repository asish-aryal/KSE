using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinect_Explorer
{
    class ViewManager
    {
        private ViewerWindow vWindow;

        private double aspect_ratio = (0.6125);
        private Controller controller;
        private int current_item_width;
        private Package_ current_package;
        //private bool browse_dialogue_displayed = false;
        private bool doc_loaded = false;
        private static bool isInstantiated = false;
        private ItemLocationManager item_location_manager;
        private KinectHandler kinectHandler;
        private int max_zoom_height;
        private int max_zoom_width;
        private int min_zoom;
        private Package_ root;
        //private static ViewerWindow viewer_window;
        private Ellipse pointer_right;
        private Ellipse pointer_left;
        private Brush selection_color;

        private ViewManager()
        {
            
            controller = new Controller();
            item_location_manager = new ItemLocationManager();

            current_item_width = 150;
            min_zoom = 150;
            doc_loaded = false;
            selection_color = Brushes.Beige;
            initialise_pointers();
        }


        private void initialise_pointers()
        {
            pointer_right = new Ellipse();
            //pointer_left = new Ellipse();
            pointer_right.Width = 20;
            //pointer_left.Width = 20;
            pointer_right.Height = 20;
            //pointer_left.Height = 20;
            pointer_right.Fill = Brushes.Green;
            //pointer_left.Fill = Brushes.Blue;
            pointer_right.Opacity = 0.5;
            //pointer_left.Opacity = 0.5;
            Canvas.SetRight(pointer_right, 0);
            //Canvas.SetLeft(pointer_left, 0);
            Canvas.SetBottom(pointer_right, 0);
            //Canvas.SetBottom(pointer_left, 0);
            vWindow.main_view.Children.Add(pointer_right);
            //main_view.Children.Add(pointer_left);
            Canvas.SetZIndex(this.pointer_right, 2);
            //pointer.Visibility = Visibility.Hidden;

        }

    }
}
