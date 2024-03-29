﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinect_Explorer
{
    class ViewManager : Observer
    {
        private ViewerWindow vWindow;

        private double aspect_ratio = (0.6125);
        private Controller controller;
        private int current_item_width;
        private Package_ current_package;
        private bool browse_dialogue_displayed = false;
        private bool doc_loaded = false;
        private static bool isInstantiated = false;
        private ItemLocationManager item_location_manager;
        private KinectHandler kinectHandler;
        private int max_zoom_height;
        private int max_zoom_width;
        private int min_zoom;
        private Package_ root;
        private static ViewManager viewManager;
        private Ellipse pointer_right;
        private Ellipse pointer_left;
        private Brush selection_color;

        private GestureManager gManager;
        private SpeechManager sManager;

        private ViewManager()
        {
            vWindow = new ViewerWindow();
            vWindow.Attach(this);
            vWindow.Show();
            controller = new Controller();
            item_location_manager = new ItemLocationManager();

            current_item_width = 150;
            min_zoom = 150;
            doc_loaded = false;
            selection_color = Brushes.LightGreen;
            initialise_pointers();
            kinectHandler = new KinectHandler(vWindow.kinectSensorChooser);

            gManager = kinectHandler.getGestureManager();
            sManager = kinectHandler.getSpeechManager();
            gManager.Attach(this);
            sManager.Attach(this);
            //repaint();


        }

        public override void Update()
        {
            if (vWindow.IsLoaded)
            {
                updatePointer(gManager.getPointerPosition(vWindow.main_view.ActualWidth, vWindow.main_view.ActualHeight), gManager.getPointerDiameter());
                update_selection_from_pointer();
                repaint();
                updateStatusIcons();
            }


        }

        private void updatePointer(Point position, double diameter)
        {
            Canvas.SetLeft(pointer_right, (int)(position.X - diameter/4));
            Canvas.SetTop(pointer_right, (int)(position.Y - diameter/4));
            //vWindow.main_view.Children.Add(pointer_right);
        }

        private void updateStatusIcons()
        {
            //For Gestures
            if (gManager.State == GRStates.CANNOT_SEE)
            { vWindow.gesture_status.Source = new BitmapImage(new Uri("./Resources/images/gesture_not_ready.png", UriKind.Relative)); }
            else if (gManager.State == GRStates.GETTING_READY)
            { vWindow.gesture_status.Source = new BitmapImage(new Uri("./Resources/images/gesture_waiting.png", UriKind.Relative)); }
            else if (gManager.State == GRStates.LOOKING)
            { vWindow.gesture_status.Source = new BitmapImage(new Uri("./Resources/images/gesture_ready.png", UriKind.Relative)); }

            //For Speech
            if (sManager.Status == SRStates.NOT_LISTENING)
            { vWindow.speech_status.Source = new BitmapImage(new Uri("./Resources/images/mic_not_ready.png", UriKind.Relative)); }
            else if (sManager.Status == SRStates.LISTENING)
            { vWindow.speech_status.Source = new BitmapImage(new Uri("./Resources/images/mic_ready.png", UriKind.Relative)); }
            else if (sManager.Status == SRStates.GETTING_READY)
            { vWindow.speech_status.Source = new BitmapImage(new Uri("./Resources/images/mic_waiting.png", UriKind.Relative)); }

        }

        public static ViewManager getInstance()
        {
            if (isInstantiated)
            { return viewManager; }
            else
            {
                viewManager = new ViewManager();
                isInstantiated = true;
                return viewManager;
            }
        }

        public void start()
        {
            kinectHandler.start();
            
            Keyboard.Focus(vWindow.help);
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

        private void display_speech_suggestions()
        {

            vWindow.speech_suggestion_value.Inlines.Add("Browse");

        }


        public Ellipse get_pointer_right()
        { return pointer_right; }

        public Ellipse get_pointer_left()
        { return pointer_left; }

        

        public TextBlock get_speech_suggestion_block()
        { return vWindow.speech_suggestion_value; }

        public Canvas getMainCanvas()
        { return vWindow.main_view; }

        public Brush SelectionColor
        {
            get { return selection_color; }
            set { selection_color = value; }
        }

        public Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer get_color_viewer()
        { return vWindow.color_viewer; }

        public Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser getKSC()
        { return vWindow.kinectSensorChooser; }

        public void closeWindow()
        {
            vWindow.Close();
        }

        public void load_root_package()
        {
            Package_ temp = null;
            if (browse_dialogue_displayed == false)
            {
                browse_dialogue_displayed = true;
                temp = controller.get_package();
                browse_dialogue_displayed = false;
            }
            if (temp != null)
            {
                root = temp;
                current_package = root;
                doc_loaded = true;
                current_item_width = min_zoom;
                repaint();



            }

        }

        public void move_selection_down()
        {
            for (int i = 0; i < item_location_manager.ITEMS_PER_COLUMN; i++)
            { move_selection_right(); }
        }

        public void move_selection_left()
        {
            if (((item_location_manager.SELECTED_ITEM - 1) > 0))
            {
                item_location_manager.SELECTED_ITEM -= 1;
                repaint();
            }
        }

        public void move_selection_right()
        {
            if (((item_location_manager.SELECTED_ITEM + 1) <= (current_package.Children_Packages.Count + current_package.Children_Classifiers.Count)))
            {
                item_location_manager.SELECTED_ITEM += 1;
                repaint();
            }
        }

        public void move_selection_up()
        {
            for (int i = 0; i < item_location_manager.ITEMS_PER_COLUMN; i++)
            { move_selection_left(); }
        }

        public void next_page()
        {
            for (int i = 0; i < item_location_manager.ITEMS_PER_PAGE; i++)
            { move_selection_right(); }
        }

        public void previous_page()
        {
            for (int i = 0; i < item_location_manager.ITEMS_PER_PAGE; i++)
            { move_selection_left(); }
        }

        public void zoom_in()
        {
            zoom_in(1.02);
        }

        public void zoom_in(double factor)
        {
            if (!doc_loaded)
            { return; }
            int proposed_zoom = (int)((double)current_item_width * factor);
            if ((proposed_zoom <= max_zoom_width) && (proposed_zoom * aspect_ratio <= max_zoom_height))
            {
                current_item_width = proposed_zoom;

                repaint();
            }
            else if ((proposed_zoom > max_zoom_width) && (proposed_zoom * aspect_ratio <= max_zoom_height) && (isClassifier(item_location_manager.SELECTED_ITEM)))
            {
                current_item_width = max_zoom_width;
                repaint();
            }
            else if ((proposed_zoom <= max_zoom_width) && (proposed_zoom * aspect_ratio > max_zoom_height) && (isClassifier(item_location_manager.SELECTED_ITEM)))
            {
                current_item_width = (int)((max_zoom_height) / aspect_ratio);
                repaint();
            }

            else if ((item_location_manager.ITEMS_PER_PAGE == 1) && (isPackage(item_location_manager.SELECTED_ITEM)))
            {
                current_package = current_package.Children_Packages[item_location_manager.CURRENT_PAGE - 1];
                current_item_width = min_zoom;
                repaint();
            }

        }

        public void update_selection_from_pointer()
        {
            //pointer_right.Visibility = pointer_left.Visibility = Visibility.Visible;



            double from_top = Canvas.GetTop(pointer_right);
            double from_left = Canvas.GetLeft(pointer_right);

            for (int i = 0; i < item_location_manager.ITEMS_PER_PAGE; i++)
            {
                double y_pos = item_location_manager.get_point((item_location_manager.CURRENT_PAGE - 1) * item_location_manager.ITEMS_PER_PAGE + i + 1).Y;
                double x_pos = item_location_manager.get_point((item_location_manager.CURRENT_PAGE - 1) * item_location_manager.ITEMS_PER_PAGE + i + 1).X;
                if (((from_top > y_pos) && (from_top < (y_pos + current_item_width * aspect_ratio))) && ((from_left > x_pos) && (from_left < (x_pos + current_item_width))))
                {
                    item_location_manager.SELECTED_ITEM = (item_location_manager.CURRENT_PAGE - 1) * item_location_manager.ITEMS_PER_PAGE + i + 1;
                    //repaint();
                    break;
                }
            }

        }

        public void zoom_out()
        { zoom_out(1.02); }

        public void zoom_out(double factor)
        {
            if (!doc_loaded)
            { return; }
            int proposed_zoom = (int)((double)current_item_width / factor);
            if ((current_item_width == min_zoom) && (current_package.Parent != null))
            {
                Package_ pkg;
                for (int i = 0; i < current_package.Parent.Children_Packages.Count; i++)
                {
                    pkg = current_package.Parent.Children_Packages[i];
                    if (pkg.Equals(current_package))
                    {
                        item_location_manager.SELECTED_ITEM = i + 1;
                        break;
                    }
                }
                current_package = current_package.Parent;
                if (max_zoom_width * aspect_ratio > max_zoom_height)
                {
                    current_item_width = (int)((double)max_zoom_height / aspect_ratio);
                }
                else
                {
                    current_item_width = max_zoom_width;
                }
                repaint();
            }
            else if (proposed_zoom > min_zoom)
            {
                current_item_width = proposed_zoom;
                repaint();
            }
            else if (proposed_zoom <= min_zoom)
            {
                current_item_width = min_zoom;
                repaint();
            }


        }
        // Private Methods (24) 


        private void draw_box(double top_left_X, double top_left_Y, double width, double height, Brush brush, Boolean fill)
        {
            Rectangle class_fig = new Rectangle();
            class_fig.Width = width;
            class_fig.Height = height;
            class_fig.Stroke = brush;
            if (fill)
            { class_fig.Fill = brush; }
            class_fig.StrokeThickness = 2;
            Canvas.SetTop(class_fig, top_left_Y);
            Canvas.SetLeft(class_fig, top_left_X);
            Canvas.SetZIndex(class_fig, 1);
            vWindow.main_view.Children.Add(class_fig);
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
            draw_box(top_left_X, top_left_Y, width, name_height, Brushes.Black, false);
            draw_box(top_left_X, top_left_Y, width, variables_height + name_height, Brushes.Black, false);
            draw_box(top_left_X, top_left_Y, width, methods_height + variables_height + name_height, Brushes.Black, false);

        }

        private void draw_class_info(double top_left_X, double top_left_Y, Classifier_ classifier, int width)
        {

            TextBlock class_name = new TextBlock();
            TextBlock class_fields = new TextBlock();
            TextBlock class_methods = new TextBlock();
            class_name.Width = width * 0.9;
            class_fields.Width = class_methods.Width = width * 0.975;
            //class_fields.Background = Brushes.White;

            class_fields.Height = class_methods.Height = width / 4 - 2;

            if (classifier.Name.Length <= 15)
            { class_name.Text = classifier.Name; }
            else
            { class_name.Text = classifier.Name.Substring(0, 12) + "..."; }

            String field_text = "[no fields]";
            if (classifier.Fields.Count > 0)
            { field_text = ""; }
            int field_count = classifier.Fields.Count;
            foreach (Field_ field in classifier.Fields)
            {

                if (field.Visibility == Member_Level_Visibility.PRIVATE)
                { field_text += "-"; }
                else if (field.Visibility == Member_Level_Visibility.PUBLIC)
                { field_text += "+"; }
                else
                { field_text += "#"; }
                field_text += field.Name + " : " + field.Type;
                if (!(field.Equals(classifier.Fields[field_count - 1])))
                { field_text += Environment.NewLine; }
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

            class_name.FontFamily = new FontFamily("Courier New");
            class_fields.TextWrapping = class_methods.TextWrapping = TextWrapping.Wrap;
            Canvas.SetLeft(class_name, top_left_X + width / 20);
            Canvas.SetLeft(class_fields, top_left_X + width / 80);
            Canvas.SetLeft(class_methods, top_left_X + width / 80);

            Canvas.SetTop(class_name, top_left_Y);
            Canvas.SetTop(class_fields, top_left_Y + width / 8);
            Canvas.SetTop(class_methods, top_left_Y + width * 3 / 8);

            Canvas.SetZIndex(class_name, 1);
            Canvas.SetZIndex(class_fields, 1);
            Canvas.SetZIndex(class_methods, 1);


            class_name.FontSize = width / 10;

            if (width < 300)
            { class_fields.FontSize = class_methods.FontSize = width / 20; }
            else
            { class_fields.FontSize = class_methods.FontSize = 300 / 20; }

            if (width >= 150)
            {
                vWindow.main_view.Children.Add(class_methods);
                vWindow.main_view.Children.Add(class_fields);
            }
            vWindow.main_view.Children.Add(class_name);
        }

        private void draw_diagrams()
        {
            List<Classifier_> classifiers = current_package.Children_Classifiers;
            List<Package_> packages = current_package.Children_Packages;
            int total_items = classifiers.Count + packages.Count;
            item_location_manager.recalcuate((int)vWindow.main_view.ActualWidth, (int)vWindow.main_view.ActualHeight, current_item_width, (int)(current_item_width * aspect_ratio), total_items);


            if (item_location_manager.TOTAL_PAGES < item_location_manager.CURRENT_PAGE)
            { item_location_manager.CURRENT_PAGE = item_location_manager.TOTAL_PAGES; }
            int no_of_packages = packages.Count;
            int no_of_classifiers = classifiers.Count;

            Point current_point;
            for (int i = 0; i < (no_of_classifiers + no_of_packages); i++)
            {
                //calculation to check if i points to an item in the current page
                if ((i >= (item_location_manager.CURRENT_PAGE - 1) * item_location_manager.ITEMS_PER_PAGE) && (i < item_location_manager.CURRENT_PAGE * item_location_manager.ITEMS_PER_PAGE))
                {
                    current_point = item_location_manager.get_point(i + 1);

                    //draw the selection outline
                    if ((i + 1) == item_location_manager.SELECTED_ITEM)
                    {
                        draw_box(current_point.X - current_item_width / 30, current_point.Y - current_item_width / 30, current_item_width + current_item_width / 15, current_item_width * aspect_ratio + current_item_width / 15, selection_color, true);
                    }

                    //draw the diagram of item pointed to by the pointer (package or class)
                    if (i < no_of_packages)
                    { draw_package(current_point.X, current_point.Y, packages[i].Name, current_item_width); }
                    else if ((i - no_of_packages) < no_of_classifiers)
                    {
                        draw_class(current_point.X, current_point.Y, classifiers[i - no_of_packages], current_item_width);
                    }



                }
            }



            vWindow.main_view.Children.Add(pointer_right);
            //main_view.Children.Add(pointer_left);

            vWindow.page_info.Content = "Page " + item_location_manager.CURRENT_PAGE + " of " + item_location_manager.TOTAL_PAGES; ;
            vWindow.depth_info.Content = "Package depth:   " + get_package_heirarchy(current_package); ;
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
            draw_box(top_left_X, top_left_Y, width / 3, name_height, Brushes.Black, false);
            draw_box(top_left_X, top_left_Y + name_height - 2, width, description_height, Brushes.Black, false);
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
            Canvas.SetZIndex(package_name_block, 1);
            package_name_block.FontSize = width / 10;
            vWindow.main_view.Children.Add(package_name_block);
        }

        
        private String get_package_heirarchy(Package_ package)
        {
            if (package.Parent != null)
            { return get_package_heirarchy(package.Parent) + " > " + package.Name; }
            else
            { return package.Name; }
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

        public void keyboard_input(object sender, KeyEventArgs e)
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
                else if (e.Key == Key.W)
                {
                    move_selection_up();
                }
                else if (e.Key == Key.S)
                { move_selection_down(); }
                else if (e.Key == Key.A)
                { move_selection_left(); }
                else if (e.Key == Key.D)
                { move_selection_right(); }

                Keyboard.Focus(vWindow.help);

            }
        }

        



        private void repaint()
        {
            if (doc_loaded)
            {
                max_zoom_width = (int)vWindow.main_view.ActualWidth - 50;
                max_zoom_height = (int)vWindow.main_view.ActualHeight - 50;
                vWindow.main_view.Children.Clear();
                draw_diagrams();

                if (vWindow.kinectSensorChooser.Kinect != null)
                {
                    vWindow.speech_suggestion_value.Inlines.Clear();
                    if ((!isClassifier(item_location_manager.SELECTED_ITEM)) || (!((max_zoom_width == current_item_width) || ((max_zoom_height - current_item_width * aspect_ratio) <= 2))))
                    { vWindow.speech_suggestion_value.Inlines.Add("Zoom in" + Environment.NewLine); }
                    if ((current_package.Name.ToUpper() != "ROOT") || (current_item_width != min_zoom))
                    { vWindow.speech_suggestion_value.Inlines.Add("Zoom out" + Environment.NewLine); }
                    if (item_location_manager.CURRENT_PAGE < item_location_manager.TOTAL_PAGES)
                    { vWindow.speech_suggestion_value.Inlines.Add("Next Page" + Environment.NewLine); }
                    if (item_location_manager.CURRENT_PAGE > 1)
                    { vWindow.speech_suggestion_value.Inlines.Add("Previous Page" + Environment.NewLine); }
                }
            }
            else
            {
                String msg = "";
                TextBlock browse = new TextBlock();
                 msg += "Press B ";
                if(sManager.Status == SRStates.LISTENING)
                { msg += "or say \"Browse\" "; }
                msg += "to open an XML file";
                browse.Text = msg;
                browse.FontSize = 40;
                vWindow.main_view.Children.Clear();
                vWindow.main_view.Children.Add(browse);
                Canvas.SetLeft(browse, vWindow.main_view.ActualWidth / 2 - browse.Width/2);
                Canvas.SetTop(browse, vWindow.main_view.ActualHeight / 2 - browse.ActualHeight / 2);
            }
        }

        private bool isClassifier(int item_number)
        {
            if ((item_number > current_package.Children_Packages.Count) && (item_number > 0))
            { return true; }
            else
            { return false; }

        }

        private bool isPackage(int item_number)
        {
            if ((item_number <= current_package.Children_Packages.Count) && (item_number > 0))
            { return true; }
            else
            { return false; }
        }

        public void stopKinect()
        {
            kinectHandler.StopKinect();
        }



    }
}
