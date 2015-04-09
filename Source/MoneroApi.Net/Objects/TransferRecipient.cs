using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TransferRecipient
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        public ulong Amount { get; set; }

        public TransferRecipient(string address, ulong amountAtomicValue)
        {
            Address = address;
            Amount = amountAtomicValue;
        }

        public TransferRecipient(string address, double amountDisplayValue)
        {
            Address = address;
            Amount = Utilities.CoinDisplayValueToAtomicValue(amountDisplayValue);
        }
    }
}
