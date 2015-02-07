using System;

namespace Jojatekok.MoneroAPI
{
    public class LogMessageReceivedEventArgs : EventArgs
    {
        public LogMessage LogMessage { get; private set; }

        internal LogMessageReceivedEventArgs(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }
    }
}
