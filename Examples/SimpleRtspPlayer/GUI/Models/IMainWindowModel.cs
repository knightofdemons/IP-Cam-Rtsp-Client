using System;
using RtspClientSharp;

namespace SimpleRtspPlayer.GUI.Models
{
    interface IMainWindowModel
    {
        event EventHandler<string> StatusChanged;

        IVideoSource VideoSource { get; }
        IVideoSource VideoSource2 { get; }

        void Start(ConnectionParameters connectionParameters, ConnectionParameters connectionParameters2);
        void Stop();
    }
}