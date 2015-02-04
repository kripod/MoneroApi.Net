using Jojatekok.MoneroAPI.ProcessManagers;
using System;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;

namespace Jojatekok.MoneroAPI
{
    /// <summary>Provides properties used by the API, and methods for value conversion.</summary>
    public static class Utilities
    {
        private const double CoinAtomicValueDivider = 1000000000000;
        private const int CoinDisplayValueDecimalPlaces = 12;

        public const string DefaultRpcUrlHostDaemon = "http://localhost";
        public const string DefaultRpcUrlHostAccountManager = "http://localhost";
        public const ushort DefaultRpcUrlPortDaemon = 18081;
        public const ushort DefaultRpcUrlPortAccountManager = 18082;

        public const int DefaultTimerSettingRpcCheckAvailabilityPeriod = 1000;
        public const int DefaultTimerSettingRpcCheckAvailabilityDueTime = 5000;
        public const int DefaultTimerSettingDaemonQueryNetworkInformationPeriod = 750;
        public const int DefaultTimerSettingAccountRefreshPeriod = 10000;

        public const int DefaultTransactionMixCount = 1;

        internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static double CoinAtomicValueToDisplayValue(ulong atomicValue)
        {
            return Math.Round(atomicValue / CoinAtomicValueDivider, CoinDisplayValueDecimalPlaces);
        }

        public static ulong CoinDisplayValueToAtomicValue(double displayValue)
        {
            return (ulong)Math.Round(displayValue * CoinAtomicValueDivider);
        }

        internal static DateTime UnixTimestampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
    }
}
