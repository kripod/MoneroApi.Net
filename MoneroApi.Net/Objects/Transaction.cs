namespace Jojatekok.MoneroAPI
{
    public class Transaction
    {
        public string TransactionId { get; internal set; }

        public ulong AmountSpendable { get; internal set; }
        public ulong AmountUnspendable { get; internal set; }

        public uint Index { get; internal set; }

        // TODO: Replace this with a date
        public uint Number { get { return Index + 1; } }
    }
}
