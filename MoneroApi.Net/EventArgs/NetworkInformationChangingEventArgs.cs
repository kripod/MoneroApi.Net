using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;

namespace Jojatekok.MoneroAPI
{
    public class NetworkInformationChangingEventArgs : ValueChangingEventArgs<NetworkInformation>
    {
        internal NetworkInformationChangingEventArgs(NetworkInformation newValue, NetworkInformation oldValue) : base(newValue, oldValue)
        {

        }
    }
}
