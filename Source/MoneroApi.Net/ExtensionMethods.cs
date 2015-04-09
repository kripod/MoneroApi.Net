using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Jojatekok.MoneroAPI
{
    public static class ExtensionMethods
    {
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        internal static T DeserializeObject<T>(this JsonSerializer serializer, string value)
        {
            if (value == null) return default(T);

            using (var stringReader = new StringReader(value)) {
                using (var jsonTextReader = new JsonTextReader(stringReader)) {
                    return (T)serializer.Deserialize(jsonTextReader, typeof(T));
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        internal static string SerializeObject<T>(this JsonSerializer serializer, T value)
        {
            using (var stringWriter = new StringWriter(Utilities.InvariantCulture)) {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter)) {
                    serializer.Serialize(jsonTextWriter, value);
                }

                return stringWriter.ToString();
            }
        }

        internal static void StartImmediately(this Timer timer, int period)
        {
            if (timer == null) return;
            timer.Change(0, period);
        }

        internal static void StartOnce(this Timer timer, int dueTime)
        {
            if (timer == null) return;
            timer.Change(dueTime, Timeout.Infinite);
        }

        public static void Stop(this Timer timer)
        {
            if (timer == null) return;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
