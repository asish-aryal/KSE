using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Kinect_Explorer
{
    class SpeechManager : Observable
    {
        private SRStates status;
        private TextBlock speech_suggestion_block;
        private Image speech_status_icon;
        private ViewManager vManager;
        private KinectSensor _sensor;
        private DispatcherTimer readyTimer;
        private EnergyCalculatingPassThroughStream stream;

        private SpeechRecognitionEngine speechRecognizer;


        public SpeechManager(KinectSensor ks)
        {
            this._sensor = ks;
            //this.vManager = vm;
            
            //speech_status_icon = vManager.get_speech_status_icon();

        }

        public SRStates Status
        {
            get { return status; }
        }

        public void updateSensor(KinectSensor sensor)
        {
            _sensor = sensor;
        }

        public void start()
        {
            
            vManager = ViewManager.getInstance();
            speech_suggestion_block = vManager.get_speech_suggestion_block();
            speechRecognizer = CreateSpeechRecognizer();
            if (speechRecognizer != null)
            {
                // NOTE: Need to wait 4 seconds for device to be ready to stream audio right after initialization
                this.readyTimer = new DispatcherTimer();
                this.readyTimer.Tick += this.ReadyTimerTick;
                this.readyTimer.Interval = new TimeSpan(0, 0, 4);
                this.readyTimer.Start();
                changeState(SRStates.GETTING_READY);
                //this.UpdateInstructionsText(string.Empty);

                //this.Closing += this.MainWindowClosing;
            }

        }

        private void changeState(SRStates changedState)
        {
            if (status != changedState)
            {
                status = changedState;
                Notify();
            }
        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            
            //this.Start();
            //speech_status_icon.Source = new BitmapImage(new Uri("./Resources/images/mic_ready.png", UriKind.Relative));
            
            //status_block.Text = "Alright, Hit Me!";

            this.readyTimer.Stop();
            this.readyTimer = null;
            
            changeState(SRStates.LISTENING);
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
            grammar.Add("Select");
            grammar.Add("Exit Application");
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

            switch (e.Result.Text.ToUpperInvariant())
            {
                case "ZOOM IN":
                    vManager.zoom_in(2);
                    break;
                case "ZOOM OUT":
                    vManager.zoom_out(1.8);
                    break;
                case "NEXT PAGE":
                    vManager.next_page();
                    break;
                case "PREVIOUS PAGE":
                    vManager.previous_page();
                    break;
                case "SELECT":
                    //System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    break;
                case "EXIT APPLICATION":
                    vManager.closeWindow();
                    break;
                case "BROWSE":
                    vManager.load_root_package();
                    break;
                default:
                    break;
            }
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


    }
}
