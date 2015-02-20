using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI
{
    public class AccountBalance
    {
        [JsonProperty("balance")]
        public ulong Total { get; private set; }

        [JsonProperty("unlocked_balance")]
        public ulong Spendable { get; private set; }

        public ulong? Unconfirmed {
            get { return Total - Spendable; }
        }

        public AccountBalance(ulong total, ulong spendable)
        {
            Total = total;
            Spendable = spendable;
        }

        public static bool operator ==(AccountBalance a, AccountBalance b)
        {
            if (ReferenceEquals(a, b)) return true;
            if ((object)a == null ^ (object)b == null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(AccountBalance a, AccountBalance b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var b = obj as AccountBalance;
            return (object)b != null && Equals(b);
        }

        public bool Equals(AccountBalance b)
        {
            if (b == null) return false;
            return
                Total == b.Total &&
                Spendable == b.Spendable
            ;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
