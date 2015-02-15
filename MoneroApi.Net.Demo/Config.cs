using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;

namespace Jojatekok.MoneroAPI.Demo
{
    struct Config
    {
        // This file's changes should not be tracked by git.
        // In order to disable tracking, execute the following command in your shell:
        // git update-index --assume-unchanged "MoneroApi.Net.Demo/Config.cs"

        public const bool IsDaemonProcessRemote = false;
        public const bool IsAccountManagerProcessRemote = false;

        public static readonly IRpcSettings ClientRpcSettings = new RpcSettings();
        public static readonly ITimerSettings ClientTimerSettings = new TimerSettings();

        public static readonly DaemonProcessSettings ClientDaemonProcessSettings = new DaemonProcessSettings();
        public static readonly AccountManagerProcessSettings ClientAccountManagerProcessSettings = new AccountManagerProcessSettings();
    }
}
