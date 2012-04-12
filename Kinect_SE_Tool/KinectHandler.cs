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

namespace Kinect_SE_Tool
{
    class KinectHandler
    {
        private KinectSensor _sensor;
        private ViewerWindow vWindow;
        private SpeechRecognitionEngine speechRecognizer;
        private EnergyCalculatingPassThroughStream stream;
        private DispatcherTimer readyTimer;
        private TextBlock speech_block;
        private TextBlock status_block;
        KinectColorViewer color_viewer;

        private GestureManager gManager;



        public KinectHandler()
        { 
           vWindow = ViewerWindow.getInstance();
           speech_block = vWindow.get_speech_block();
           status_block = vWindow.get_status_block();
           Image color_view = vWindow.get_image_frame();
           color_viewer = vWindow.get_color_viewer();
           gManager = new GestureManager(_sensor);
        }


        public void start()
        {



            if (KinectSensor.KinectSensors.Count > 0)
            { _sensor = KinectSensor.KinectSensors[0]; }
            else
            {
                status_block.Text = "Kinect not connected :(";
                return; 
            }
            initializeKinect(_sensor);



            
        }

        private class EnergyCalculatingPassThroughStream : Stream
        {
            private const int SamplesPerPixel = 10;

            private readonly double[] energy = new double[500];
            private readonly object syncRoot = new object();
            private readonly Stream baseStream;

            private int index;
            private int sampleCount;
            private double avgSample;

            public EnergyCalculatingPassThroughStream(Stream stream)
            {
                this.baseStream = stream;
            }

            public override long Length
            {
                get { return this.baseStream.Length; }
            }

            public override long Position
            {
                get { return this.baseStream.Position; }
                set { this.baseStream.Position = value; }
            }

            public override bool CanRead
            {
                get { return this.baseStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return this.baseStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return this.baseStream.CanWrite; }
            }

            public override void Flush()
            {
                this.baseStream.Flush();
            }

            public void GetEnergy(double[] energyBuffer)
            {
                lock (this.syncRoot)
                {
                    int energyIndex = this.index;
                    for (int i = 0; i < this.energy.Length; i++)
                    {
                        energyBuffer[i] = this.energy[energyIndex];
                        energyIndex++;
                        if (energyIndex >= this.energy.Length)
                        {
                            energyIndex = 0;
                        }
                    }
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int retVal = this.baseStream.Read(buffer, offset, count);
                const double A = 0.3;
                lock (this.syncRoot)
                {
                    for (int i = 0; i < retVal; i += 2)
                    {
                        short sample = BitConverter.ToInt16(buffer, i + offset);
                        this.avgSample += sample * sample;
                        this.sampleCount++;

                        if (this.sampleCount == SamplesPerPixel)
                        {
                            this.avgSample /= SamplesPerPixel;

                            this.energy[this.index] = .2 + ((this.avgSample * 11) / (int.MaxValue / 2));
                            this.energy[this.index] = this.energy[this.index] > 10 ? 10 : this.energy[this.index];

                            if (this.index > 0)
                            {
                                this.energy[this.index] = (this.energy[this.index] * A) + ((1 - A) * this.energy[this.index - 1]);
                            }

                            this.index++;
                            if (this.index >= this.energy.Length)
                            {
                                this.index = 0;
                            }

                            this.avgSample = 0;
                            this.sampleCount = 0;
                        }
                    }
                }

                return retVal;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                this.baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.baseStream.Write(buffer, offset, count);
            }
        }

        void sensor_chooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor oldSensor = (KinectSensor)e.OldValue;
            StopKinect(oldSensor);

            if ((KinectSensor)e.NewValue != null)
            {
                _sensor = (KinectSensor)e.NewValue;
                initializeKinect(_sensor);
            }
            //throw new NotImplementedException();
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
                    JitterRadius = 0.5f,
                    MaxDeviationRadius = 0.5f
                };
                kinect_sensor.SkeletonStream.Enable(parameters);


                kinect_sensor.ColorStream.Enable();
                //kinect_sensor.SkeletonStream.Enable();
                kinect_sensor.DepthStream.Enable();
                kinect_sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);



                speechRecognizer = CreateSpeechRecognizer();
                
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


                if (speechRecognizer != null && kinect_sensor != null)
                {
                    // NOTE: Need to wait 4 seconds for device to be ready to stream audio right after initialization
                    this.readyTimer = new DispatcherTimer();
                    this.readyTimer.Tick += this.ReadyTimerTick;
                    this.readyTimer.Interval = new TimeSpan(0, 0, 4);
                    this.readyTimer.Start();

                    status_block.Text = "Hold on... I'm getting ready";
                    //this.UpdateInstructionsText(string.Empty);

                    //this.Closing += this.MainWindowClosing;
                }

            }
        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            //this.Start();
            status_block.Text = "Alright, Hit Me!";
            
            this.readyTimer.Stop();
            this.readyTimer = null;

            var audioSource = _sensor.AudioSource;
            //audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = audioSource.Start();
            this.stream = new EnergyCalculatingPassThroughStream(kinectStream);
            this.speechRecognizer.SetInputToAudioStream(
                this.stream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }


    private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.Ensure you have the Microsoft Speech SDK installed.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                return null;
            }

            SpeechRecognitionEngine sre;
            try
            {
                sre = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed and configured.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                return null;
            }

            var grammar = new Choices();
            grammar.Add("red");
            grammar.Add("green");
            grammar.Add("blue");
            grammar.Add("Camera on");
            grammar.Add("Camera off");
            grammar.Add("Zoom in");
            grammar.Add("Zoom out");
            grammar.Add("Browse");
            grammar.Add("Previous Page");
            grammar.Add("Next Page");
            grammar.Add("I Kill You");
            grammar.Add("Default");


            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            sre.LoadGrammar(g);
            sre.SpeechRecognized += this.SreSpeechRecognized;
            sre.SpeechHypothesized += this.SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;

            return sre;
        }

    private void RejectSpeech(RecognitionResult result)
    {
        //MessageBox.Show("Rejected: " + (result == null ? string.Empty : result.Text + " " + result.Confidence));
    }

    private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
    {
        this.RejectSpeech(e.Result);
    }

    private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
    {
        //MessageBox.Show("Hypothesized: " + e.Result.Text + " " + e.Result.Confidence);
    }

    private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        if (e.Result.Confidence < 0.5)
        {
            this.RejectSpeech(e.Result);
            return;
        }

        speech_block.Text = e.Result.Text;

        switch (e.Result.Text.ToUpperInvariant())
        {
            case "RED":
                speech_block.Background = Brushes.Red;
                break;
            case "GREEN":
                speech_block.Background = Brushes.Green;
                //brush = this.greenBrush;
                break;
            case "BLUE":
                speech_block.Background = Brushes.Blue;
                //brush = this.blueBrush;
                break;
            case "CAMERA ON":
                color_viewer.Visibility = Visibility.Visible;
                //System.Diagnostics.Process.Start("notepad.exe");
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Visible;
                //brush = this.blackBrush;
                break;
            case "CAMERA OFF":
                color_viewer.Visibility = Visibility.Hidden;
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Hidden;
                //brush = this.blackBrush;
                break;
            case "ZOOM IN":
                vWindow.zoom_in(2);
                //System.Diagnostics.Process.Start("notepad.exe");
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Visible;
                //brush = this.blackBrush;
                break;
            case "ZOOM OUT":
                vWindow.zoom_out(1.8);
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Hidden;
                //brush = this.blackBrush;
                break;
            case "NEXT PAGE":
                vWindow.next_page();
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Hidden;
                //brush = this.blackBrush;
                break;
            case "PREVIOUS PAGE":
                vWindow.previous_page();
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Hidden;
                //brush = this.blackBrush;
                break;
            case "I KILL YOU":
                vWindow.Close();
                //this.kinectColorViewer1.Visibility = System.Windows.Visibility.Hidden;
                //brush = this.blackBrush;
                break;
            case "BROWSE":
                vWindow.load_root_package();
                break;
            case "DEFAULT":
                speech_block.Background = Brushes.LightGray;
                break;
            default:
                //brush = this.blackBrush;
                break;
        }

        
        //MessageBox.Show("Recognized: " + e.Result.Text + " " + e.Result.Confidence);
    }
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }


        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            gManager.processFrames(e);

            //if (closing)
            //{
            //    return;
            //}

            //Get a skeleton
            
        }



        


        private void StopKinect(KinectSensor sensor)
        {
            
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }

        public void StopKinect()
        {
            //StopKinect(_sensor);
        }

    }
}
