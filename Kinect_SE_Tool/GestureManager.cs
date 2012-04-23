using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using Coding4Fun.Kinect.Wpf;
using System.Threading;

namespace Kinect_Explorer
{
    class GestureManager
    {
        ViewManager vManager = ViewManager.getInstance();
        KinectSensor sensor;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        //private List<Joint> rightHandPositions;

        int historyDuration = 2; //in seconds
        private SkeletonHistoryManager historyManager;
        //private bool zooming_in = false;

        JointType pointerJointType;


        public GestureManager(KinectSensor sensor)
        {
            this.sensor = sensor;
            historyManager= new SkeletonHistoryManager(historyDuration);
            //rightHandPositions = new List<Joint>();

            pointerJointType = JointType.HandRight;
        }


        public void processFrames(AllFramesReadyEventArgs e)
        {
            Skeleton first = GetFirstSkeleton(e);
            if (first == null)
            {
                vManager.get_gesture_status_icon().Source = new BitmapImage(new Uri("./Resources/images/gesture_not_ready.png", UriKind.Relative));
                return;
            }

            vManager.get_gesture_status_icon().Source = new BitmapImage(new Uri("./Resources/images/gesture_ready.png", UriKind.Relative));
            update_pointer(first);

            historyManager.addToHistory(first);

            if (historyManager.IsReady)
            { recogniseGestures(); }
            else
            { vManager.get_gesture_status_icon().Source = new BitmapImage(new Uri("./Resources/images/gesture_waiting.png", UriKind.Relative)); }
        }


        private void recogniseGestures()
        {
            
            checkZoomGesture();
        }

        private void checkZoomGesture()
        {
            double initial_position = historyManager.getJoint(JointType.HandLeft, 0.3).Position.Z;
            double final_position = historyManager.getJoint(JointType.HandLeft, 0).Position.Z;
            double diff = final_position - initial_position;
            //vWindow.speech_feedback_value.Text = diff.ToString();
            if (diff >= 0.25)
            {
                

                        vManager.zoom_in(1.04);

                
                //vWindow.zoom_in(1.5);
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }
            else if (diff <= -0.25)
            {
                vManager.zoom_out(1.04);
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }
        
        }


        private void update_pointer(Skeleton sk)
        {
            //set scaled position
            //ScalePosition(headImage, first.Joints[JointType.Head]);
            //ScalePosition(vWindow.get_pointer_left(), sk.Joints[JointType.HandLeft]);
            //Joint scaledJoint = first.Joints[JointType.HandLeft].ScaleTo((int)vWindow.main_view.ActualWidth - (int)vWindow.get_pointer_right().ActualWidth, (int)vWindow.main_view.ActualHeight - (int)vWindow.get_pointer_right().ActualHeight, 0.5f, 0.5f);
            //Canvas.SetLeft(vWindow.get_pointer_left(), scaledJoint.Position.X);
            //Canvas.SetTop(vWindow.get_pointer_left(), scaledJoint.Position.Y);

            //ScalePosition(vWindow.get_pointer_right(), sk.Joints[pointerJoint]);

            ScalePointerPosition(vManager.get_pointer_right(), sk);

            vManager.get_pointer_right().Width = vManager.get_pointer_right().Height = Math.Abs((int)((sk.Joints[JointType.HandLeft].Position.Z-0.8)*100));

            //GetCameraPoint(first, e);
            vManager.update_selection_from_pointer();
        }

        private void ScalePointerPosition(FrameworkElement element, Skeleton skeleton)
        {
            Joint pointerJoint = skeleton.Joints[pointerJointType];
            double scaled_X = (pointerJoint.Position.X - skeleton.Joints[JointType.Spine].Position.X) * (vManager.getMainCanvas().ActualWidth / 0.5);
            if (scaled_X < 0)
            { scaled_X = 0; }
            if (scaled_X > vManager.getMainCanvas().ActualWidth)
            { scaled_X = vManager.getMainCanvas().ActualWidth; }
            double scaled_Y = (skeleton.Joints[JointType.ShoulderRight].Position.Y - pointerJoint.Position.Y) * (vManager.getMainCanvas().ActualHeight / 0.5);
            if (scaled_Y < 0) 
            { scaled_Y = 0; }
            if (scaled_Y > vManager.getMainCanvas().ActualHeight)
            { scaled_Y = vManager.getMainCanvas().ActualHeight; }


            Canvas.SetLeft(element, (int)scaled_X- vManager.get_pointer_right().ActualWidth/2);
            Canvas.SetTop(element, (int)scaled_Y - vManager.get_pointer_right().ActualHeight/2);

        }

        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

    }
}
