using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using RtspClientSharp;
using SimpleRtspPlayer.GUI.Models;
using SimpleRtspPlayer.GUI.Views;

namespace SimpleRtspPlayer.GUI.ViewModels
{

    class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        private string _status = string.Empty;
        private readonly IMainWindowModel _mainWindowModel;
        private bool _startButtonEnabled = true;
        private bool _stopButtonEnabled;
        private bool _fullscreenEnabled = false;

        public string DeviceAddress { get; set; }

        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "123456";

        public IVideoSource VideoSource => _mainWindowModel.VideoSource;
        public IVideoSource VideoSource2 => (IVideoSource)_mainWindowModel.VideoSource2;

        public RelayCommand MouseDoubleClickCommand { get; }
        public RelayCommand StartClickCommand { get; }
        public RelayCommand StopClickCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IMainWindowModel mainWindowModel)
        {
            _mainWindowModel = mainWindowModel ?? throw new ArgumentNullException(nameof(mainWindowModel));

            StartClickCommand = new RelayCommand(OnStartButtonClick, () => _startButtonEnabled);
            StopClickCommand = new RelayCommand(OnStopButtonClick, () => _stopButtonEnabled);
//            MouseDoubleClickCommand = new RelayCommand(OnDoubleClick, () => _fullscreenEnabled); 
             ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        
        public void OnStartButtonClick()
        {
            string address = "rtsp://192.168.0.254:82/live/ch00_1";
            string address2 = "rtsp://192.168.0.254:81/live/ch00_1";

            if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                address = RtspPrefix + address;

            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri deviceUri))
            {
                MessageBox.Show("Invalid device address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!address2.StartsWith(RtspPrefix) && !address2.StartsWith(HttpPrefix))
                address2 = RtspPrefix + address2;

            if (!Uri.TryCreate(address2, UriKind.Absolute, out Uri deviceUri2))
            {
                MessageBox.Show("Invalid device address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var credential = new NetworkCredential(Login, Password);

            var connectionParameters = !string.IsNullOrEmpty(deviceUri.UserInfo) ? new ConnectionParameters(deviceUri) : 
                new ConnectionParameters(deviceUri, credential);

            connectionParameters.RtpTransport = RtpTransportProtocol.TCP;
            connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);
            
            var connectionParameters2 = !string.IsNullOrEmpty(deviceUri2.UserInfo) ? new ConnectionParameters(deviceUri2) :
                new ConnectionParameters(deviceUri2, credential);

            connectionParameters2.RtpTransport = RtpTransportProtocol.TCP;
            connectionParameters2.CancelTimeout = TimeSpan.FromSeconds(1);

            _mainWindowModel.Start(connectionParameters, connectionParameters2);
            _mainWindowModel.StatusChanged += MainWindowModelOnStatusChanged;


            _startButtonEnabled = false;
            StartClickCommand.RaiseCanExecuteChanged();
            _stopButtonEnabled = true;
            StopClickCommand.RaiseCanExecuteChanged();
        }

        private void OnStopButtonClick()
        {
            _mainWindowModel.Stop();
            _mainWindowModel.StatusChanged -= MainWindowModelOnStatusChanged;

            _stopButtonEnabled = false;
            StopClickCommand.RaiseCanExecuteChanged();
            _startButtonEnabled = true;
            StartClickCommand.RaiseCanExecuteChanged();
            Status = string.Empty;
        }
/*        private void MouseDoubleClickCommand()
        {
            if (_fullscreenEnabled)
            {

                _fullscreenEnabled = false;
                MouseDoubleClickCommand.RaiseCanExecuteChanged();
            }
            else if (!_fullscreenEnabled){

                _fullscreenEnabled = true;
                MouseDoubleClickCommand.RaiseCanExecuteChanged();
            }
        }
*/
        private void MainWindowModelOnStatusChanged(object sender, string s)
        {
            Application.Current.Dispatcher.Invoke(() => Status = s);
        }

        private void OnClosing(CancelEventArgs args)
        {
            _mainWindowModel.Stop();
        }
    }
}