using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Kinect.Wpf; 

namespace Kinect_SE_Tool
{
    class GestureManager
    {
        ViewerWindow vWindow = ViewerWindow.getInstance();
        KinectSensor sensor;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        //private List<Joint> rightHandPositions;

        //in seconds
        int historyDuration = 2;
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
            if (first == null || (vWindow.get_pointer_right() == null)) return;
            update_pointer(first);

            historyManager.addToHistory(first);

            if (historyManager.IsReady)
            { recogniseGestures(); }
        }


        private void recogniseGestures()
        {
            //if (historyManager.getJointHistory(JointType.HandRight).Equals(historyManager.getJointHistory(JointType.HandRight)[itemsInHistory - 1]))
            //{MessageBox.Show("All the items in the array refer to the same joint!");}
            checkZoomGesture();
            
        }

        private void checkZoomGesture()
        {
            double initial_position = historyManager.getJoint(JointType.HandLeft, 0.5).Position.Z;
            double final_position = historyManager.getJoint(JointType.HandLeft, 0).Position.Z;
            double diff = final_position - initial_position;
            vWindow.speech_feedback_value.Text = diff.ToString();
            if (diff >= 0.25)
            {
                vWindow.zoom_in(1.5);
                historyManager.clearHistory();
            }
            else if (diff <= -0.25)
            {
                vWindow.zoom_out(1.6);
                historyManager.clearHistory();
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

            ScalePointerPosition(vWindow.get_pointer_right(), sk);

            vWindow.get_pointer_right().Width = vWindow.get_pointer_right().Height = Math.Abs((int)((sk.Joints[JointType.HandLeft].Position.Z-0.8)*100));

            //GetCameraPoint(first, e);
            vWindow.update_selection_from_pointer();
        }

        private void ScalePointerPosition(FrameworkElement element, Skeleton skeleton)
        {
            Joint pointerJoint = skeleton.Joints[pointerJointType];
            double scaled_X = (pointerJoint.Position.X - skeleton.Joints[JointType.Spine].Position.X) * (vWindow.main_view.ActualWidth / 0.5);
            if (scaled_X < 0)
            { scaled_X = 0; }
            if (scaled_X > vWindow.main_view.ActualWidth)
            { scaled_X = vWindow.main_view.ActualWidth; }
            double scaled_Y = (skeleton.Joints[JointType.ShoulderRight].Position.Y - pointerJoint.Position.Y) * (vWindow.main_view.ActualHeight / 0.5);
            if (scaled_Y < 0) 
            { scaled_Y = 0; }
            if (scaled_Y > vWindow.main_view.ActualHeight)
            { scaled_Y = vWindow.main_view.ActualHeight; }


            Canvas.SetLeft(element, (int)scaled_X- vWindow.get_pointer_right().ActualWidth/2);
            Canvas.SetTop(element, (int)scaled_Y - vWindow.get_pointer_right().ActualHeight/2);

        }

        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //convert the value to X/Y
            //Joint scaledJoint = joint.ScaleTo(1280, 720);

            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joint.ScaleTo((int)vWindow.main_view.ActualWidth - (int)vWindow.get_pointer_right().ActualWidth, (int)vWindow.main_view.ActualHeight - (int)vWindow.get_pointer_right().ActualHeight, .2f, .2f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

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
