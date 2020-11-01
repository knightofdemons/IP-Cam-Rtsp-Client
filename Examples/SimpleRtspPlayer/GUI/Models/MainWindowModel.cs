using System;
using System.Windows;
using RtspClientSharp;
using SimpleRtspPlayer.RawFramesReceiving;

namespace SimpleRtspPlayer.GUI.Models
{

    class MainWindowModel : IMainWindowModel
    {

        private readonly RealtimeVideoSource _realtimeVideoSource = new RealtimeVideoSource();
        private readonly RealtimeAudioSource _realtimeAudioSource = new RealtimeAudioSource();
        private readonly RealtimeVideoSource _realtimeVideoSource2 = new RealtimeVideoSource();

        private IRawFramesSource _rawFramesSource;
        private IRawFramesSource _rawFramesSource2;

        public event EventHandler<string> StatusChanged;

        public IVideoSource VideoSource => _realtimeVideoSource;
        public IVideoSource VideoSource2 => (IVideoSource)_realtimeVideoSource2;

        public void Start(ConnectionParameters connectionParameters, ConnectionParameters connectionParameters2)
        {
            if (_rawFramesSource != null)
                return;

            _rawFramesSource = new RawFramesSource(connectionParameters);
            _rawFramesSource.ConnectionStatusChanged += ConnectionStatusChanged;
            _realtimeVideoSource.SetRawFramesSource(_rawFramesSource);
            _realtimeAudioSource.SetRawFramesSource(_rawFramesSource);
            _rawFramesSource.Start();

            _rawFramesSource2 = new RawFramesSource(connectionParameters2);
            _rawFramesSource2.ConnectionStatusChanged += ConnectionStatusChanged;
            _realtimeVideoSource2.SetRawFramesSource(_rawFramesSource2);
            _rawFramesSource2.Start();
        }

        public void Stop()
        {
            if (_rawFramesSource == null)
                return;

            _rawFramesSource.Stop();
            _realtimeVideoSource.SetRawFramesSource(null);
            _rawFramesSource = null;
            _rawFramesSource2.Stop();
            _realtimeVideoSource2.SetRawFramesSource(null);
            _rawFramesSource2 = null;
        }

        private void ConnectionStatusChanged(object sender, string s)
        {
            StatusChanged?.Invoke(this, s);
        }
    }
}