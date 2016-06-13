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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using NAudio.Wave;
using NAudio;
using NAudio.Mixer;
using System.Diagnostics;
using System.IO;

namespace VolumeDetector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public float maxValue = 0;
        public float minValue = 0;
        public float tenSecondAverage = 0;
        public float[] tenSecondAverageBuffer = new float[100];
        public float threshold = 10;
        public float averageSeconds = 10;
        public int microphoneIndex = 0;
        public string message = "Nathan you are being too loud!";
        public NotifyIcon notifyIcon = new NotifyIcon();

        FloatingOSDWindow warningWindow = new FloatingOSDWindow();

        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
            SetUpMicrophone();
        }

        private void ReadSettings()
        {
            try
            {
                String line = "";
                StreamReader reader = new StreamReader("Settings.ini");
                line = reader.ReadLine();
                threshold = (float)Convert.ToDouble(line);
                Threshold.Text = threshold.ToString();

                line = reader.ReadLine();
                microphoneIndex = Convert.ToInt16(line);

                line = reader.ReadLine();
                averageSeconds = (float)Convert.ToDouble(line);

                message = reader.ReadLine();

            }
            catch (Exception) { 
            
            }
        }

        public void SetUpMicrophone()
        {
            try {

                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(microphoneIndex);
                WaveIn waveIn = new WaveIn();
                waveIn.DeviceNumber = microphoneIndex;
                waveIn.DataAvailable += waveIn_DataAvailable;
                int sampleRate = 8000; // 8 kHz
                int channels = 1; // mono
                waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
                waveIn.StartRecording();
                timer.Start();
                
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + e.Message);
            }
        }

        // Handle the wave data when it comes in
        public long count = 0;
        private float lastPeak;
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;

                maxValue = Math.Max(maxValue, sample32);
                minValue = Math.Min(minValue, sample32);
                count++;

                if (count >= 441)
                {
                    lastPeak = Math.Max(minValue, Math.Abs(maxValue)) * 100;
                    VolumeLevel.Value = (Double)lastPeak;

                    //Roll the window over
                    for (int i = 0; i < averageSeconds; i++)
                    {
                        if (i == averageSeconds-1)
                        {
                            tenSecondAverageBuffer[i] = lastPeak;
                        } else {
                            tenSecondAverageBuffer[i] = tenSecondAverageBuffer[i + 1];;
                        }
                    }

                    //Work out the average over the last 10 seconds
                    for (int i = 0; i < averageSeconds; i++) tenSecondAverage += tenSecondAverageBuffer[i];
                    tenSecondAverage /= averageSeconds;
                    VolumeLevel10SecondAverage.Value = (Double)tenSecondAverage;

                    if (tenSecondAverage >= threshold) DisplayWarning(message);
 
                    count = 0;
                    maxValue = minValue = 0;
                }   
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try {
                float temp = (float)Convert.ToDouble(Threshold.Text);
                if (temp >= 0 && temp <= 100) { 
                    threshold = temp;
                } else {
                    System.Windows.Forms.MessageBox.Show("That isn't a valid number dummy, enter a number between 0 and 100%");
                }
            } catch (Exception){
                System.Windows.Forms.MessageBox.Show("That isn't a valid number dummy, enter a number between 0 and 100%");
            }

        }

        Stopwatch timer = new Stopwatch();
        private void DisplayWarning(String warning)
        {

            if (timer.ElapsedMilliseconds > 2000) timer.Stop();

            if (timer.IsRunning == false)
            {
                timer.Start();
                System.Drawing.Rectangle Screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                System.Drawing.Point point = new System.Drawing.Point((Screen.Width / 6) - 200, Screen.Height / 2 - Screen.Height / 4);
                byte alpha = 240;
                System.Drawing.Font font = new Font("Arial", 96f, System.Drawing.FontStyle.Bold);

                warningWindow.Show(point, alpha, System.Drawing.Color.Red, font, 2000, FloatingWindow.AnimateMode.SlideLeftToRight, 20, message);
            }
        }

        private void ImportStatusForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }
    }
}
