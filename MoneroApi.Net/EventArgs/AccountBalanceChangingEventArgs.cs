namespace Jojatekok.MoneroAPI
{
    public class AccountBalanceChangingEventArgs : ValueChangingEventArgs<AccountBalance>
    {
        internal AccountBalanceChangingEventArgs(AccountBalance newValue, AccountBalance oldValue) : base(newValue, oldValue)
        {

        }
    }
}
