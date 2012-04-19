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
using System.Windows.Media.Animation;

namespace Kinect_Explorer
{
    /// <summary>
    /// Interaction logic for TestWindow2.xaml
    /// </summary>
    public partial class TestWindow2 : Window
    {


        public TestWindow2()
        {
            InitializeComponent();
            enlarge_shape();
            translate_shape();

        }

        private void enlarge_shape()
        {
            Rectangle rect1 = new Rectangle();
            rect1.Width = 200;
            rect1.Height = 120;
            rect1.StrokeThickness = 3;
            rect1.Stroke = Brushes.Black;
            Canvas.SetLeft(rect1, 30);
            canvas1.Children.Add(rect1);

            DoubleAnimation da = new DoubleAnimation();
            da.From = 30;
            da.To = 100;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            //rect1.BeginAnimation(Rectangle.HeightProperty, da);
            rect1.BeginAnimation(Rectangle.HeightProperty, da);
            rect1.BeginAnimation(Rectangle.WidthProperty, da);
            
        }

        private void translate_shape()
        { 
            Rectangle rect3 = new Rectangle();
            rect3.Width = 72;
            rect3.Height = 28;
            rect3.Stroke = Brushes.Blue;
            rect3.StrokeThickness = 3;
            Canvas.SetLeft(rect3, 300);
            canvas1.Children.Add(rect3);
            TranslateTransform move_shape= new TranslateTransform();
            
            DoubleAnimation da = new DoubleAnimation();
            da.To = 500;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard myStoryboard = new Storyboard();

            
            //Rectangle.RenderTransformProperty(TranslateTransform)
        }

    }
}
