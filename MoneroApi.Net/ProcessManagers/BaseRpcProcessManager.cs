using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseRpcProcessManager : IDisposable
    {
        public event EventHandler RpcAvailabilityChanged;

        private bool _isRpcAvailable;

        private RpcWebClient RpcWebClient { get; set; }
        private ITimerSettings TimerSettings { get; set; }

        private string RpcHost { get; set; }
        private ushort RpcPort { get; set; }

        private Timer TimerCheckRpcAvailability { get; set; }

        public bool IsRpcAvailable {
            get { return _isRpcAvailable; }

            protected set {
                if (value == _isRpcAvailable) return;

                _isRpcAvailable = value;
                if (value) TimerCheckRpcAvailability.Stop();
                if (RpcAvailabilityChanged != null) RpcAvailabilityChanged(this, EventArgs.Empty);
            }
        }

        internal BaseRpcProcessManager(RpcWebClient rpcWebClient, bool isDaemon) {
            RpcWebClient = rpcWebClient;
            TimerSettings = rpcWebClient.TimerSettings;

            TimerCheckRpcAvailability = new Timer(
                delegate { CheckRpcAvailability(); },
                null,
                TimerSettings.RpcCheckAvailabilityDueTime,
                TimerSettings.RpcCheckAvailabilityPeriod
            );

            var rpcSettings = rpcWebClient.RpcSettings;

            if (isDaemon) {
                RpcHost = rpcSettings.UrlHostDaemon;
                RpcPort = rpcSettings.UrlPortDaemon;

            } else {
                RpcHost = rpcSettings.UrlHostAccountManager;
                RpcPort = rpcSettings.UrlPortAccountManager;
            }
        }

        private void CheckRpcAvailability()
        {
            // TODO: Change this behavior
            IsRpcAvailable = true;
        }

        protected T HttpPostData<T>(string command) where T : HttpRpcResponse
        {
            if (!IsRpcAvailable) return null;

            var output = RpcWebClient.HttpPostData<T>(RpcHost, RpcPort, command);
            if (output != null && output.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        protected JsonRpcResponse<T> JsonPostData<T>(JsonRpcRequest jsonRpcRequest) where T : class
        {
            if (!IsRpcAvailable) return new JsonRpcResponse<T>(new JsonError());
            return RpcWebClient.JsonPostData<T>(RpcHost, RpcPort, jsonRpcRequest);
        }

        protected void JsonPostData(JsonRpcRequest jsonRpcRequest)
        {
            JsonPostData<object>(jsonRpcRequest);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                TimerCheckRpcAvailability.Dispose();
                TimerCheckRpcAvailability = null;
            }
        }
    }
}
