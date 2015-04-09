using System;

namespace Jojatekok.MoneroAPI
{
    public class NetworkInformationChangedEventArgs : EventArgs
    {
        public NetworkInformation NetworkInformation { get; private set; }

        internal NetworkInformationChangedEventArgs(NetworkInformation networkInformation)
        {
            NetworkInformation = networkInformation;
        }
    }
}
