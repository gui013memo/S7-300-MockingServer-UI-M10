using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace S7_300_MockingServer_UI
{
    public partial class MaxwellForm : Form
    {
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioReader;

        public MaxwellForm()
        {
            InitializeComponent();
        }

        private void MaxwellForm_Load(object sender, EventArgs e)
        {
            
        }

        private void PlayAudioFromProjectFolder()
        {
            StopAndDisposeAudio(); // Clean up any previous playback

            waveOutDevice = new WaveOutEvent();

            try
            {
                string audioFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio");
                string audioFilePath = Path.Combine(audioFolderPath, "maxwell-the-cat-theme.mp3");

                if (!File.Exists(audioFilePath))
                {
                    throw new FileNotFoundException("Audio file not found.", audioFilePath);
                }

                audioReader = new AudioFileReader(audioFilePath);
                waveOutDevice.Init(audioReader);
                waveOutDevice.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MaxwellForm_Shown(object sender, EventArgs e)
        {
            PlayAudioFromProjectFolder(); 
        }

        private void MaxwellForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Prevent the form from disposing
            StopAndDisposeAudio(); // Stop audio when form is hidden
            this.Hide(); // Just hide the form
        }

        private void StopAndDisposeAudio()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }

            if (audioReader != null)
            {
                audioReader.Dispose();
                audioReader = null;
            }
        }

        private void MaxwellForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopAndDisposeAudio(); // Ensure resources are cleaned up
        }
    }
}
