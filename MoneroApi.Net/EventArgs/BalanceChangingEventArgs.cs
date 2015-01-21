using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;

namespace Jojatekok.MoneroAPI
{
    public class BalanceChangingEventArgs : ValueChangingEventArgs<Balance>
    {
        internal BalanceChangingEventArgs(Balance newValue, Balance oldValue) : base(newValue, oldValue)
        {

        }
    }
}
