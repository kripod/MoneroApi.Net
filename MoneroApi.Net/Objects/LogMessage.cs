using System;

namespace Jojatekok.MoneroAPI
{
    public class LogMessage
    {
        public string MessageText { get; private set; }
        public DateTime Time { get; private set; }

        internal LogMessage(string messageText, DateTime time)
        {
            MessageText = messageText;
            Time = time;
        }
    }
}
