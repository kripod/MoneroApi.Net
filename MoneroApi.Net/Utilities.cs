using System;
using System.Globalization;

namespace Jojatekok.MoneroAPI
{
    /// <summary>Provides properties used by the API, and methods for value conversion.</summary>
    public static class Utilities
    {
        public const int CoinDisplayValueDecimalPlaces = 12;
        private static readonly double CoinAtomicValueDivider = Math.Pow(10, CoinDisplayValueDecimalPlaces);

        public static readonly string StringFormatCoinDisplayValue = "0." + new string('0', CoinDisplayValueDecimalPlaces);

        public const string DefaultRpcUrlHost = "http://localhost";
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

        public static string CoinAtomicValueToString(ulong atomicValue)
        {
            return CoinDisplayValueToString(CoinAtomicValueToDisplayValue(atomicValue));
        }

        public static string CoinDisplayValueToString(double displayValue)
        {
            return displayValue.ToString(StringFormatCoinDisplayValue, InvariantCulture);
        }

        public static bool IsAddressValid(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length != 95 ||
                address[0] != '4' ||
                address[1] < '0' || address[1] > 'B'
            ) {
                return false;
            }

            for (var i = 2; i < address.Length; i++) {
                var currentChar = address[i];
                if (currentChar < '0' || currentChar > 'z') {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPaymentIdValid(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId)) return true;
            if (paymentId.Length != 64) return false;

            for (var i = paymentId.Length - 1; i >= 0; i--) {
                var currentChar = paymentId[i];
                if (currentChar < '0' || char.ToUpper(currentChar, InvariantCulture) > 'F') {
                    return false;
                }
            }

            return true;
        }

        internal static DateTime UnixTimestampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
    }
}
