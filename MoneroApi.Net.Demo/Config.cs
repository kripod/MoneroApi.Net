using Jojatekok.MoneroAPI.Settings;

namespace Jojatekok.MoneroAPI.Demo
{
    struct Config
    {
        // This file's changes should not be tracked by git

        public static readonly IRpcSettings ClientRpcSettings = new RpcSettings();
        public static readonly IPathSettings ClientPathSettings = new PathSettings();
        public static readonly ITimerSettings ClientTimerSettings = new TimerSettings();
    }
}
