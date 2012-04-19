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
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Kinect_Explorer
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        Storyboard myStoryboard;
        public TestWindow()
        {
            InitializeComponent();
            create_rectangles();
        }

        private void create_rectangle()
        {
            Rectangle rect = new Rectangle();
            Canvas.SetRight(rect, 5);
            rect.Name = "Code_Rectangle";
            this.RegisterName(rect.Name, rect);
            rect.Stroke = Brushes.Black;
            rect.StrokeThickness = 5;
            //rect.Fill = Brushes.Blue;
            rect.Width = 200;
            rect.Height = 120;
            
            
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 200;
            myDoubleAnimation.To = 250;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();
            myDoubleAnimation2.From = 120;
            myDoubleAnimation2.To = 150;
            myDoubleAnimation2.Duration = new Duration(TimeSpan.FromSeconds(1));
            myDoubleAnimation2.AutoReverse = true;
            myDoubleAnimation2.RepeatBehavior = RepeatBehavior.Forever;

            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimation2);
            Storyboard.SetTargetName(myDoubleAnimation, rect.Name);
            Storyboard.SetTargetName(myDoubleAnimation2, rect.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Rectangle.WidthProperty));
            Storyboard.SetTargetProperty(myDoubleAnimation2, new PropertyPath(Rectangle.HeightProperty));
            
            canvas1.Children.Add(rect);
        }

        private void animate(object sender, RoutedEventArgs e)
        {
            //this.UnregisterName("rectangle3");
            myStoryboard.Begin(this);
        }

        private void create_rectangles()
        {
            ArrayList myShapes = new ArrayList();
            Rectangle myShape;
            DoubleAnimation myDoubleAnimation;
            for (int i = 0; i < 1; i++)
            {
                myShape = new Rectangle();
                Canvas.SetLeft(myShape, i * 220 + 20);
                Canvas.SetTop(myShape, 15);
                myShape.Name = "rectangle" + i.ToString();
                //MessageBox.Show(myShape.Name);
                this.RegisterName(myShape.Name, myShape);
                myShape.Stroke = Brushes.Black;
                myShape.StrokeThickness = 5;
                myShape.Width = 200;
                myShape.Height = 120;
                
                
                

                myDoubleAnimation = new DoubleAnimation();
                //myDoubleAnimation.From = 200;
                myDoubleAnimation.To = 250;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;


                myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                Storyboard.SetTargetName(myDoubleAnimation, myShape.Name);
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Rectangle.HeightProperty));
                canvas1.Children.Add(myShape);
            }

            
        }



    }
}
