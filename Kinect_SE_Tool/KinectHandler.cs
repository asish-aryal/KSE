using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Samples.Kinect.WpfViewers;
using Coding4Fun.Kinect.Wpf; 
using System.Windows.Threading;

namespace Kinect_Explorer
{
    class KinectHandler
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensor _sensor;
        //private ViewManager vManager;
        //private TextBlock speech_suggestion_block;
        private Image speech_status_icon;
        private GestureManager gManager;
        private SpeechManager sManager;


        public KinectHandler(KinectSensorChooser sensor_chooser)
        {
            sensorChooser = sensor_chooser;
            _sensor = sensor_chooser.Kinect;
           //vManager = ViewManager.getInstance();
           //speech_suggestion_block = vManager.get_speech_suggestion_block();
           //speech_status_icon = vManager.get_speech_status_icon();
           //Image color_view = vWindow.get_image_frame();
           gManager = new GestureManager(_sensor);
           sManager = new SpeechManager(_sensor);
        }

        public SpeechManager getSpeechManager()
        {
            return sManager;
        }

        public GestureManager getGestureManager()
        {
            return gManager;
        }


        public void start()
        {
            sensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
        }

        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor oldSensor = (KinectSensor)e.OldValue;
            StopKinect(oldSensor);

            if ((KinectSensor)e.NewValue != null)
            {
                _sensor = (KinectSensor)e.NewValue;
                initializeKinect(_sensor);
            }
        }





        private void initializeKinect(KinectSensor kinect_sensor)
        {
            if (kinect_sensor.Status == KinectStatus.Connected)
            {

                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.3f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 0.2f,
                    MaxDeviationRadius = 0.5f
                };
                kinect_sensor.SkeletonStream.Enable(parameters);

                kinect_sensor.ColorStream.Enable();
                kinect_sensor.DepthStream.Enable();
                kinect_sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                
                try
                {
                    kinect_sensor.Start();
                    //kinect_sensor.AudioSource.Start();
                }
                catch (System.IO.IOException)
                {
                    new System.IO.IOException("The Kinect is being used by another Application");
                    //throw;
                }

                if (kinect_sensor != null)
                {
                    sManager.updateSensor(_sensor);
                    sManager.start();
                }

            }
        }

        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            gManager.processFrames(e);   
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                
                sensor.Stop();
                sensor.AudioSource.Stop();
                sensor.Dispose();
            }
        }

        public void StopKinect()
        {
            StopKinect(_sensor);
        }

    }
}
