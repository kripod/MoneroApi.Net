using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI
{
    public class AccountBalance
    {
        [JsonProperty("balance")]
        public ulong? Total { get; private set; }

        [JsonProperty("unlocked_balance")]
        public ulong? Spendable { get; private set; }

        public ulong? Unconfirmed {
            get {
                if (Total == null || Spendable == null) return null;
                return Total.Value - Spendable.Value;
            }
        }

        public AccountBalance(ulong? total, ulong? spendable)
        {
            Total = total;
            Spendable = spendable;
        }
    }
}
