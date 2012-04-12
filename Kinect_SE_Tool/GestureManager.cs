﻿using System;
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
        int itemsInHistory=10;
        private SkeletonHistoryManager historyManager;
        //private bool zooming_in = false;


        public GestureManager(KinectSensor sensor)
        {
            this.sensor = sensor;
            historyManager= new SkeletonHistoryManager(itemsInHistory);
            //rightHandPositions = new List<Joint>();
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
            if (historyManager.getJointHistory(JointNames.Joints.HandRight).Equals(historyManager.getJointHistory(JointNames.Joints.HandRight)[itemsInHistory - 1]))
            {MessageBox.Show("All the items in the array refer to the same joint!");}
            double initial_position = historyManager.getJointHistory(JointNames.Joints.HandLeft)[0].Position.Z;
            double final_position = historyManager.getJointHistory(JointNames.Joints.HandLeft)[itemsInHistory - 1].Position.Z;
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
            ScalePosition(vWindow.get_pointer_right(), sk.Joints[JointType.HandRight]);

            vWindow.get_pointer_right().Width = vWindow.get_pointer_right().Height = Math.Abs((int)((sk.Joints[JointType.HandLeft].Position.Z-0.8)*100));

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