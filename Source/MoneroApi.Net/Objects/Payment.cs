using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI
{
    public class Payment
    {
        [JsonProperty("payment_id")]
        public string PaymentId { get; private set; }

        [JsonProperty("tx_hash")]
        public string TransactionId { get; private set; }

        [JsonProperty("block_height")]
        public ulong BlockHeight { get; private set; }

        [JsonProperty("amount")]
        public ulong Amount { get; private set; }

        [JsonProperty("unlock_time")]
        public ulong UnlockTime { get; private set; }
    }
}
