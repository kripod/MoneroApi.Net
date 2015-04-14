using System;
using System.Globalization;

namespace Jojatekok.MoneroAPI
{
    /// <summary>Provides properties used by the API, and methods for value conversion.</summary>
    public static class Utilities
    {
        private const int ValidAddressLength = 95;
        private const int ValidPaymentIdLength = 64;

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

        public static string GenerateRandomPaymentId()
        {
            var random = new Random();
            var array = new byte[ValidPaymentIdLength >> 1];
            random.NextBytes(array);

            return ByteArrayToString(array);

        }

        public static bool IsAddressValid(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length != ValidAddressLength ||
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
            if (paymentId.Length != ValidPaymentIdLength) return false;

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

        private static string ByteArrayToString(byte[] input)
        {
            var output = new char[input.Length << 1];

            for (var i = input.Length - 1; i >= 0; i--) {
                var iX2 = i << 1;

                var tmp = input[i] & 0xF;
                output[iX2 + 1] = (char)(87 + tmp + (((tmp - 10) >> 31) & -39));

                tmp = input[i] >> 4;
                output[iX2] = (char)(87 + tmp + (((tmp - 10) >> 31) & -39));
            }

            return new string(output);
        }
    }
}
