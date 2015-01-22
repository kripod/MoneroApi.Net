using Newtonsoft.Json;
using System;

namespace Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses
{
    public class NetworkInformation : HttpRpcResponse
    {
        private ulong _blockHeightTotal;

        [JsonProperty("alt_blocks_count")]
        public ulong AlternativeBlockCount { get; private set; }

        [JsonProperty("difficulty")]
        public ulong BlockDifficulty { get; private set; }

        [JsonProperty("target_height")]
        public ulong BlockHeightTotal {
            get { return _blockHeightTotal; }
            private set { _blockHeightTotal = value != 0 && value < BlockHeightDownloaded ? BlockHeightDownloaded : value; }
        }
        [JsonProperty("height")]
        public ulong BlockHeightDownloaded { get; private set; }
        public ulong BlockHeightRemaining {
            get { return BlockHeightTotal - BlockHeightDownloaded; }
        }

        public DateTime BlockTimeCurrent { get; internal set; }
        public TimeSpan BlockTimeRemaining {
            get { return DateTime.UtcNow.Subtract(BlockTimeCurrent); }
        }

        [JsonProperty("grey_peerlist_size")]
        public ulong PeerListSizeGrey { get; private set; }
        [JsonProperty("white_peerlist_size")]
        public ulong PeerListSizeWhite { get; private set; }

        [JsonProperty("incoming_connections_count")]
        public ulong ConnectionCountIncoming { get; private set; }
        [JsonProperty("outgoing_connections_count")]
        public ulong ConnectionCountOutgoing { get; private set; }
        public ulong ConnectionCountTotal {
            get { return ConnectionCountIncoming + ConnectionCountOutgoing; }
        }

        [JsonProperty("tx_count")]
        public ulong TransactionCountTotal { get; private set; }
        [JsonProperty("tx_pool_size")]
        public ulong TransactionPoolSize { get; private set; }

        public static bool operator ==(NetworkInformation a, NetworkInformation b)
        {
            if (ReferenceEquals(a, b)) return true;
            if ((object)a == null ^ (object)b == null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(NetworkInformation a, NetworkInformation b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var b = obj as NetworkInformation;
            return (object)b != null && Equals(b);
        }

        public bool Equals(NetworkInformation b)
        {
            if (b == null) return false;
            return
                AlternativeBlockCount == b.AlternativeBlockCount &&
                BlockDifficulty == b.BlockDifficulty &&
                BlockHeightTotal == b.BlockHeightTotal &&
                BlockHeightDownloaded == b.BlockHeightDownloaded &&
                PeerListSizeGrey == b.PeerListSizeGrey &&
                PeerListSizeWhite == b.PeerListSizeWhite &&
                ConnectionCountIncoming == b.ConnectionCountIncoming &&
                ConnectionCountOutgoing == b.ConnectionCountOutgoing &&
                TransactionCountTotal == b.TransactionCountTotal &&
                TransactionPoolSize == b.TransactionPoolSize
            ;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
