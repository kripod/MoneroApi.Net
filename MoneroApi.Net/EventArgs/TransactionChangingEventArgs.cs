namespace Jojatekok.MoneroAPI
{
    public class TransactionChangingEventArgs : ValueChangingEventArgs<Transaction>
    {
        public int TransactionIndex { get; private set; }

        internal TransactionChangingEventArgs(Transaction newValue, Transaction oldValue, int transactionIndex) : base(newValue, oldValue)
        {
            TransactionIndex = transactionIndex;
        }
    }
}
