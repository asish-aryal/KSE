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
        private List<Skeleton> skeletonHistory;

        private bool zooming_in = false;


        public GestureManager(KinectSensor sensor)
        {
            this.sensor = sensor;
            
        }


        public void processFrames(AllFramesReadyEventArgs e)
        {
            Skeleton first = GetFirstSkeleton(e);
            update_pointer(first);

            
            
        }

        private void update_pointer(Skeleton sk)
        {
            

            if (sk == null || (vWindow.get_pointer_right() == null)) return;



            //set scaled position
            //ScalePosition(headImage, first.Joints[JointType.Head]);
            //ScalePosition(vWindow.get_pointer_left(), sk.Joints[JointType.HandLeft]);
            //Joint scaledJoint = first.Joints[JointType.HandLeft].ScaleTo((int)vWindow.main_view.ActualWidth - (int)vWindow.get_pointer_right().ActualWidth, (int)vWindow.main_view.ActualHeight - (int)vWindow.get_pointer_right().ActualHeight, 0.5f, 0.5f);
            //Canvas.SetLeft(vWindow.get_pointer_left(), scaledJoint.Position.X);
            //Canvas.SetTop(vWindow.get_pointer_left(), scaledJoint.Position.Y);
            ScalePosition(vWindow.get_pointer_right(), sk.Joints[JointType.HandRight]);

            //GetCameraPoint(first, e);
            vWindow.update_selection_from_pointer();
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
