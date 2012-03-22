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

namespace Kinect_SE_Tool
{
    /// <summary>
    /// Interaction logic for TempWindow.xaml
    /// </summary>
    public partial class ViewerWindow : Window
    {
        private static ViewerWindow viewer_window;
        private static bool isInstantiated = false;

        private bool doc_loaded = false;
        private Package_ root;
        private Package_ current_package;
        private List<String> package_depth;
        
        
        private int current_page;
        private int total_pages;
        List<PointCollection> item_positions_per_page;

        private int zoom_value;
        private int max_zoom_width;
        private int max_zoom_height;
        private int min_zoom;

        KinectHandler kinectHandler;
        Controller controller;
        
        
        private ViewerWindow()
        {
            InitializeComponent();
            controller = new Controller();
            

            zoom_value = 150;
            total_pages = 1;
            current_page = 1;
            //max_zoom = (int)main_view.ActualWidth -50;
            min_zoom = 150;
            doc_loaded = false;
            package_depth = new List<string>();
            package_depth.Add("root");
        }

        public static ViewerWindow getInstance()
        {
            if (isInstantiated)
            { return viewer_window; }
            else
            {
                viewer_window = new ViewerWindow();
                isInstantiated = true;
                return viewer_window;
            }
        }

        private void repaint()
        {
            if (doc_loaded)
            {
                max_zoom_width = (int)main_view.ActualWidth - 50;
                max_zoom_height = (int)main_view.ActualHeight - 50;
                main_view.Children.Clear();
                draw_diagrams();
            }
        }


        private void draw_diagrams()
        {
            List<Classifier_> classifiers = current_package.Children_Classifiers;
            List<Package_> packages = current_package.Children_Packages;

            int total_items = classifiers.Count + packages.Count;
            
            set_item_positions((int)main_view.ActualWidth, (int)main_view.ActualHeight, zoom_value, (int)(zoom_value * 5 / 8), total_items);
            List<PointCollection> pages_of_positions = item_positions_per_page;
            total_pages = pages_of_positions.Count;
            if (total_pages < current_page)
            { current_page = total_pages; }
            int no_of_packages = packages.Count;
            int no_of_classifiers = classifiers.Count;
            int cur_package_ind = 0;
            int cur_classifier = 0;


            for (int i = 0; i < pages_of_positions.Count; i++)
            {
                for (int j = 0; j < pages_of_positions[i].Count; j++)
                {
                    if (cur_package_ind < no_of_packages)
                    {
                        if (current_page == (i + 1))
                        { draw_package(pages_of_positions[i][j].X, pages_of_positions[i][j].Y, packages[cur_package_ind].Name, zoom_value); }
                        cur_package_ind++;
                    }
                    else if (cur_classifier < no_of_classifiers)
                    {
                        if (current_page == (i + 1))
                        { draw_class(pages_of_positions[i][j].X, pages_of_positions[i][j].Y, classifiers[cur_classifier], zoom_value); }
                        cur_classifier++;
                    }
                }
            }
            page_info.Content = "Page " + current_page + " of " + total_pages;

            string pkg_depth = "";
            string level_name;
            for(int i=0; i<package_depth.Count; i++)
            {
                level_name = package_depth[i];
                pkg_depth += level_name.ToUpper();
                if (package_depth.Count != (i + 1))
                { pkg_depth += " > "; }

            }
            depth_info.Content = "Package depth:   " + pkg_depth;
        }


        private void set_item_positions(int Canvas_Width, int Canvas_Height, int item_width, int item_height, int no_of_items )
        {
            List<PointCollection> pages = new List<PointCollection>();
            int vertical_separation = get_separation_distance(item_height, Canvas_Height);
            int horizontal_separation = get_separation_distance(item_width, Canvas_Width);

            PointCollection points_in_page;
            int cur_x;
            int cur_y;

            int cur_item = 1;
            while (cur_item <= no_of_items)
            {
                points_in_page = new PointCollection();
                
                cur_y = vertical_separation;
                while ((cur_y + item_height) < Canvas_Height) 
                {
                    cur_x = horizontal_separation;
                    while ((cur_x + item_width) < Canvas_Width)
                    {
                        points_in_page.Add(new Point((double)cur_x, (double)cur_y));
                        cur_x = cur_x + item_width + horizontal_separation;
                        cur_item++;
                    }

                    cur_y = cur_y + item_height + vertical_separation;
                }
                pages.Add(points_in_page);
            }

            item_positions_per_page = pages;
        }


        private void draw_package(double top_left_X, double top_left_Y, string name_of_class, int width)
        {
            draw_package_frame(top_left_X, top_left_Y, width);
            draw_package_info(top_left_X, top_left_Y, name_of_class, width);
        }

        private void draw_package_frame(double top_left_X, double top_left_Y, int width)
        {
            int name_height = width / 8;
            int description_height = width / 2;
            draw_box(top_left_X, top_left_Y, width / 3, name_height);
            draw_box(top_left_X, top_left_Y + name_height - 2, width, description_height);
        }

        private void draw_package_info(double top_left_X, double top_left_Y, string name_of_package, int width)
        {
            TextBlock package_name_block = new TextBlock();
            package_name_block.TextWrapping = TextWrapping.Wrap;
            package_name_block.TextAlignment = TextAlignment.Center;
            package_name_block.Width = width * 0.9;
            package_name_block.Height = (width / 2) * 0.9;
            int character_limit = 60;
            if (name_of_package.Length <= character_limit)
            {
                package_name_block.Text = name_of_package;
            }
            else
            {
                package_name_block.Text = name_of_package.Substring(0, character_limit - 3) + "...";
            }

            package_name_block.FontFamily = new FontFamily("Courier New");
            Canvas.SetLeft(package_name_block, top_left_X + width * 0.05);
            Canvas.SetTop(package_name_block, top_left_Y + (width / 8) * 1.05);
            package_name_block.FontSize = width / 10;
            main_view.Children.Add(package_name_block);
        }

        private void draw_class(double top_left_X, double top_left_Y, Classifier_ classifier, int width)
        {
            draw_class_frame(top_left_X, top_left_Y, width);
            draw_class_info(top_left_X, top_left_Y, classifier, width);
        }




        private void draw_class_frame(double top_left_X, double top_left_Y, int width)
        {

            int name_height = width / 8;
            int variables_height = width / 4;
            int methods_height = width / 4;
            draw_box(top_left_X, top_left_Y, width, name_height);
            draw_box(top_left_X, top_left_Y, width, variables_height + name_height);
            draw_box(top_left_X, top_left_Y, width, methods_height + variables_height + name_height);

        }


        private void draw_class_info(double top_left_X, double top_left_Y, Classifier_ classifier, int width)
        {
            TextBlock class_name = new TextBlock();
            TextBlock class_fields = new TextBlock();
            TextBlock class_methods = new TextBlock();
            class_name.Width = width * 0.9;
            class_fields.Width = class_methods.Width = width * 0.975;
            //class_fields.Background = Brushes.White;

            class_fields.Height = class_methods.Height = width / 4 -2;

            if (classifier.Name.Length <= 15)
            {class_name.Text = classifier.Name;}
            else
            {class_name.Text = classifier.Name.Substring(0, 12) + "...";}

            String field_text = "[no fields]";
            if (classifier.Fields.Count > 0)
            { field_text = "";}
            int field_count = classifier.Fields.Count;
            foreach (Field_ field in classifier.Fields)
            {
                    
                if (field.Visibility == Member_Level_Visibility.PRIVATE)
                { field_text += "-"; }
                else if (field.Visibility == Member_Level_Visibility.PUBLIC)
                { field_text += "+"; }
                else
                {field_text += "#";  }
                field_text += field.Name + " : " + field.Type;
                if (!(field.Equals(classifier.Fields[field_count - 1])))
                { field_text += Environment.NewLine;}
            }
            class_fields.Text = field_text;


            String method_text = "[no methods]";
            if (classifier.Methods.Count > 0)
            { method_text = ""; }
            int method_count = classifier.Methods.Count;
            foreach (Method_ method in classifier.Methods)
            {
                if (method.Visibility == Member_Level_Visibility.PRIVATE)
                { method_text += "-"; }
                else if (method.Visibility == Member_Level_Visibility.PUBLIC)
                { method_text += "+"; }
                else
                { method_text += "#"; }
                method_text += method.Name + "(";
                int no_of_params = method.Parameters.Count;
                foreach (Variable_ param in method.Parameters)
                {
                    method_text += param.Name + " : " + param.Type;
                    if (!(param.Equals(method.Parameters[no_of_params - 1])))
                    { method_text += ", "; }
                }
                method_text += ") : " + method.ReturnType;
                if (!(method.Equals(classifier.Methods[method_count - 1])))
                { method_text += Environment.NewLine; }

            }
            class_methods.Text = method_text;
            
            class_name.FontFamily =  new FontFamily("Courier New");
            class_fields.TextWrapping = class_methods.TextWrapping = TextWrapping.Wrap;
            Canvas.SetLeft(class_name, top_left_X + width / 20);
            Canvas.SetLeft(class_fields, top_left_X + width / 80);
            Canvas.SetLeft(class_methods, top_left_X + width / 80);

            Canvas.SetTop(class_name, top_left_Y);
            Canvas.SetTop(class_fields, top_left_Y + width/8);
            Canvas.SetTop(class_methods, top_left_Y + width*3/8 );


            class_name.FontSize = width / 10;

            if (width < 300)
            { class_fields.FontSize = class_methods.FontSize = width / 20; }
            else
            { class_fields.FontSize = class_methods.FontSize = 300 / 20; }

            if (width >= 150)
            {
                main_view.Children.Add(class_methods);
                main_view.Children.Add(class_fields);
            }
            main_view.Children.Add(class_name);
        }

        private void draw_box(double top_left_X, double top_left_Y, int width, int height)
        {
            Rectangle class_fig = new Rectangle();
            class_fig.Width = width;
            class_fig.Height = height;
            class_fig.Stroke = Brushes.Black;
            class_fig.StrokeThickness = 2;
            Canvas.SetTop(class_fig, top_left_Y);
            Canvas.SetLeft(class_fig, top_left_X);
            main_view.Children.Add(class_fig);
        }

        private int get_separation_distance(int space_taken_by_frame, int total_space)
        {
            int separation;
            int free_space = total_space % space_taken_by_frame;
            int no_of_gaps = (total_space / space_taken_by_frame) + 1;

            separation = free_space / no_of_gaps;

            while ((separation < 20) && (no_of_gaps > 2))
            {
                free_space = free_space + space_taken_by_frame;
                no_of_gaps--;
                separation = free_space / no_of_gaps;
            }

            return separation;
        }

        private int get_items_per_column()
        {
            int class_frame_width = zoom_value;
            int class_frame_height = class_frame_width * 5 / 8;
            int height_of_canvas = (int)main_view.ActualHeight;
            int vertical_separation = get_separation_distance(class_frame_height, height_of_canvas);
            return height_of_canvas / (class_frame_height + vertical_separation);
        }

        private int get_items_per_row()
        {
            int class_frame_width = zoom_value;
            int width_of_canvas = (int)main_view.ActualWidth;
            int horizontal_separation = get_separation_distance(class_frame_width, width_of_canvas);
            return width_of_canvas / (class_frame_width + horizontal_separation);
        }



        public void zoom_in()
        {
            zoom_in(1.05);
        }


        public void zoom_in(double factor)
        {
            if (!doc_loaded)
            { return; }
            int proposed_zoom = (int)((double)zoom_value * factor);
            if ((proposed_zoom <= max_zoom_width) && (proposed_zoom * 5 / 8 <= max_zoom_height))
            { 
                zoom_value = proposed_zoom;
                repaint();
            }
            else if ((item_positions_per_page[current_page-1].Count == 1) && (current_package.Children_Packages.Count >= current_page))
            {
                current_package = current_package.Children_Packages[current_page - 1];
                package_depth.Add(current_package.Name);
                zoom_value = min_zoom;
                repaint();
            }
            
        }

        public void zoom_out()
        { zoom_out(1.05); }

        public void zoom_out(double factor)
        {
            if (!doc_loaded)
            { return; }
            int proposed_zoom = (int)((double)zoom_value / factor);
            if ((zoom_value == min_zoom) && (current_package.Parent != null))
            {
                //current_page = current_package.Parent.Children_Packages.FindIndex(current_package) + 1;
                Package_ pkg;
                for (int i = 0; i < current_package.Parent.Children_Packages.Count; i++)
                {
                    pkg = current_package.Parent.Children_Packages[i];
                    if (pkg.Equals(current_package))
                    {
                        current_page = i + 1;
                        break;
                    }
                }
                current_package = current_package.Parent;
                package_depth.RemoveAt(package_depth.Count - 1);
                if (max_zoom_width * 5 / 8 > max_zoom_height)
                {
                    zoom_value = max_zoom_height * 8 / 5;
                }
                else
                {
                    zoom_value = max_zoom_width;
                }
                repaint();
            }
            else if (proposed_zoom > min_zoom)
            {
                zoom_value = proposed_zoom;
                repaint();
            }
            else if (proposed_zoom < min_zoom)
            {
                zoom_value = min_zoom;
                repaint();
            }
            
            
        }

        public void previous_page()
        {
            if (current_page > 1)
            { current_page--; }
            else
            { current_page = 1; }
            repaint();
        }

        public void next_page()
        {
            if (current_page < total_pages)
            { current_page++; }
            else
            { current_page = total_pages; }
            repaint();
        }


        private void keyboard_input(object sender, KeyEventArgs e)
        {
            Button source = e.Source as Button;
            if (source != null)
            {
                if (e.Key == Key.Add)
                {
                    zoom_in();
                }
                else if (e.Key == Key.Subtract)
                {
                    zoom_out();
                }
                else if (e.Key == Key.PageUp)
                {
                    previous_page();
                }
                else if (e.Key == Key.PageDown)
                {
                    next_page();
                }
                else if (e.Key == Key.B)
                {
                    load_root_package();
                }

            }
        }

        public void load_root_package()
        {
            Package_ temp = controller.get_package();
            if (temp != null)
            {
                root = temp;
                current_package = root;
                doc_loaded = true;
                zoom_value = min_zoom;
                current_page = 1;
                package_depth = new List<String>();
                package_depth.Add("root");
            }
            repaint();
        }

        private void zoom_out_Click(object sender, RoutedEventArgs e)
        {
            zoom_out();
        }

        private void zoom_in_Click(object sender, RoutedEventArgs e)
        {
            zoom_in();
        }



        private void browse_for_file(object sender, RoutedEventArgs e)
        {
            load_root_package();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            repaint();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectHandler = new KinectHandler();
            kinectHandler.start();

            //Thread kHandler = new Thread(new ThreadStart(kinectHandler.start));
            //kHandler.SetApartmentState(ApartmentState.STA);
            //kHandler.Start();
            //while (!kHandler.IsAlive) ;



            //if (KinectSensor.KinectSensors.Count > 0)
            //{
            //    KinectSensor newSensor = KinectSensor.KinectSensors[0];
            //    newSensor.ColorStream.Enable();
            //    newSensor.DepthStream.Enable();
            //    newSensor.SkeletonStream.Enable();
            //    newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
            //    try
            //    {
            //        newSensor.Start();
            //    }
            //    catch (System.IO.IOException)
            //    {
            //        kinectSensorChooser1.AppConflictOccurred();

            //    }
            //}


            Keyboard.Focus(zoom_out_button);
            
        }

        private void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //KinectSensor oldSensor = (KinectSensor)e.OldValue;
            //StopKinect(oldSensor);

            //KinectSensor newSensor = (KinectSensor)e.NewValue;
            //if (newSensor != null)
            //{
            //    colour_image_mask.Opacity = 0;
            //    skeleton_mask.Opacity = 0;
            //    newSensor.ColorStream.Enable();
            //    newSensor.DepthStream.Enable();
            //    newSensor.SkeletonStream.Enable();
            //    newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);

            //    try
            //    {
            //        newSensor.Start();
            //    }
            //    catch (System.IO.IOException)
            //    {
            //        kinectSensorChooser1.AppConflictOccurred();

            //    }
            //}
            //else
            //{
            //    colour_image_mask.Opacity = 1;
            //    skeleton_mask.Opacity = 1;
            //}


            //throw new NotImplementedException();
        }

        public Image get_image_frame()
        { return color_view; }

        public TextBlock get_status_block()
        { return Status_Value; }

        public TextBlock get_speech_block()
        { return speech_feedback_value; }

        public Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer get_color_viewer()
        { return color_viewer; }



        //public Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser get_sensor_chooser()
        //{ return kinectSensorChooser1; }

        private void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Image img = new Image();
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;
                color_view.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 92, 92, PixelFormats.Bgr32, null, pixels, stride);

            }

            //throw new NotImplementedException();
        }



        private void StopKinect(KinectSensor sensor)
        {
            //if (sensor != null)
            //{
            //    sensor.Stop();
            //    //sensor.AudioSource.Stop();

            //}
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            isInstantiated = false;
            kinectHandler.StopKinect();
            MainWindow window = new MainWindow();
            window.Show();
        }


        




    }
}
