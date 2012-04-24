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
    class GestureManager : Observable
    {
        private GRStates state;
        ViewManager vManager;
        KinectSensor sensor;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        //private List<Joint> rightHandPositions;

        int historyDuration = 2; //in seconds
        private SkeletonHistoryManager historyManager;
        //private bool zooming_in = false;
        JointType pointerJointType;
        private Gestures previousGesture;
        private DateTime lastGestureTime = DateTime.Now;



        public GestureManager(KinectSensor sensor)
        {
            
            this.sensor = sensor;
            historyManager= new SkeletonHistoryManager(historyDuration);
            //rightHandPositions = new List<Joint>();

            pointerJointType = JointType.HandRight;
        }

        public GRStates State
        {
            get { return state; }
        }

        public void processFrames(AllFramesReadyEventArgs e)
        {
            vManager = ViewManager.getInstance();
            Skeleton firstSkeleton = GetFirstSkeleton(e);
            if (firstSkeleton == null)
            {

                changeState(GRStates.CANNOT_SEE);

                return;
            }

            //vManager.get_gesture_status_icon().Source = new BitmapImage(new Uri("./Resources/images/gesture_ready.png", UriKind.Relative));
            //update_pointer(firstSkeleton);

            historyManager.addToHistory(firstSkeleton);

            if (historyManager.IsReady)
            { 
                recogniseGestures();
                changeState(GRStates.LOOKING);
            }
            else
            { changeState(GRStates.GETTING_READY);}
            Notify();
        }

        private void changeState(GRStates changedState)
        {
            if (state != changedState)
            {
                state = changedState;
                Notify();
            }
        }

        private void recogniseGestures()
        {
            
            checkZoomGesture();
            checkSwipeGesture();
        }

        private void checkSwipeGesture()
        {
            if ((DateTime.Now - lastGestureTime).Seconds < 1)
            { return; }
            double initial_position = historyManager.getJoint(JointType.HandLeft, 0.2).Position.X;
            double final_position = historyManager.getJoint(JointType.HandLeft, 0).Position.X;
            double diff = final_position - initial_position;
            if (diff >= 0.2)
            {
                vManager.previous_page();
                lastGestureTime = DateTime.Now;
                //vWindow.zoom_in(1.5);
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }
            else if (diff <= -0.2)
            {
                vManager.next_page();
                lastGestureTime = DateTime.Now;
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }


        }

        private void checkZoomGesture()
        {
            double initial_position = historyManager.getJoint(JointType.HandLeft, 0.3).Position.Z;
            double final_position = historyManager.getJoint(JointType.HandLeft, 0).Position.Z;
            double diff = final_position - initial_position;
            //vWindow.speech_feedback_value.Text = diff.ToString();
            if (diff >= 0.25)
            {

                if (!((previousGesture == Gestures.ZoomOut) && ((DateTime.Now - lastGestureTime).Seconds < 1)))
                { 
                    vManager.zoom_in(1.04);
                    lastGestureTime = DateTime.Now;
                    previousGesture = Gestures.ZoomIn;
                
                }

                
                //vWindow.zoom_in(1.5);
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }
            else if (diff <= -0.25)
            {
                if (!((previousGesture == Gestures.ZoomIn) && ((DateTime.Now - lastGestureTime).Seconds < 1)))
                { 
                    vManager.zoom_out(1.04);
                    lastGestureTime = DateTime.Now;
                    previousGesture = Gestures.ZoomOut;
                }
                //Thread.Sleep(1000);
                //historyManager.clearHistory();
            }
        
        }




        public double getPointerDiameter()
        {
            if (historyManager.IsReady)
            {
                return Math.Abs((int)((historyManager.getJoint(JointType.HandLeft,0).Position.Z - 0.8) * 100)); 
                
            }
            else
            { return 30; }
        }

        public Point getPointerPosition(double scaleTo_X, double scaleTo_Y)
        {
            //scaleTo_X = scaleTo_X - getPointerDiameter();
            //scaleTo_Y = scaleTo_Y - getPointerDiameter();
            if (!historyManager.IsReady)
            { return new Point(scaleTo_X, scaleTo_Y); }
            Joint pointerJoint = historyManager.getJoint(pointerJointType, 0);// skeleton.Joints[pointerJointType];
            double scaled_X = (pointerJoint.Position.X - historyManager.getJoint(JointType.Spine, 0).Position.X) * (scaleTo_X/ 0.5);
            if (scaled_X < 0)
            { scaled_X = 0; }
            if (scaled_X > scaleTo_X)
            { scaled_X = scaleTo_X; }
            double scaled_Y = (historyManager.getJoint(JointType.ShoulderRight, 0).Position.Y - pointerJoint.Position.Y) * (scaleTo_Y / 0.5);
            if (scaled_Y < 0)
            { scaled_Y = 0; }
            if (scaled_Y > scaleTo_Y)
            { scaled_Y = scaleTo_Y; }

            return new Point(scaled_X, scaled_Y);

            //Canvas.SetLeft(element, (int)scaled_X - vManager.get_pointer_right().ActualWidth / 2);
            //Canvas.SetTop(element, (int)scaled_Y - vManager.get_pointer_right().ActualHeight / 2);
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
