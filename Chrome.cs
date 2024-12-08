using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using WebSocket4Net;

namespace GenXdev.Helpers
{
    public class Chrome
    {
        public static object padLock = new object();

        const string JsonPostfix = "/json";

        public string remoteDebuggingUri;
        public string sessionWSEndpoint;

        public int Port { get; private set; }
        public Chrome(string remoteDebuggingUri)
        {
            this.remoteDebuggingUri = remoteDebuggingUri;
            this.Port = (new Uri(this.remoteDebuggingUri)).Port;
        }

        public TRes SendRequest<TRes>()
        {
            using (HttpClient client = new HttpClient())
            {
                return (Task.Run(async () =>
                {
                    string url = remoteDebuggingUri + JsonPostfix;
                    var response = await client.GetStringAsync(url);
                    return Deserialise<TRes>(response);
                }).Result);
            }
        }

        public List<RemoteSessionsResponse> GetAvailableSessions()
        {
            var res = this.SendRequest<List<RemoteSessionsResponse>>();
            return (from r in res
                    where r.devtoolsFrontendUrl != null
                    select r).ToList();
        }

        public string NavigateTo(string uri)
        {
            // Page.navigate is working from M18
            //var json = @"{""method"":""Page.navigate"",""params"":{""url"":""http://www.seznam.cz""},""id"":1}";

            // Instead of Page.navigate, we can use document.location
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":" + Serialization.ToJson("document.location='" + uri + "'") + @",""objectGroup"":""console"",""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
            return this.SendCommand(json);
        }

        public string GetElementsByTagName(string tagName)
        {
            // Page.navigate is working from M18
            //var json = @"{""method"":""Page.navigate"",""params"":{""url"":""http://www.seznam.cz""},""id"":1}";

            // Instead of Page.navigate, we can use document.location
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""document.getElementsByTagName('" + tagName + @"')"",""objectGroup"":""console"",""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
            return this.SendCommand(json);
        }


        public string Eval(string cmd)
        {
            return Eval(cmd, false);
        }
        public string GetEvalCommand(string cmd, bool allowTopLevelAwait = false)
        {
            string awaitParam = "";
            if (allowTopLevelAwait)
            {
                awaitParam = @"""replMode"":true,";
            }
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":" + Serialization.ToJson(cmd) + @"," + awaitParam + @"""objectGroup"":""console"",""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":true,""returnByValue"":true},""id"":1}";

            // return json;
            return json;
        }

        public string Eval(string cmd, bool allowTopLevelAwait = false, uint timeoutseconds = 30)
        {
            // return json;
            return this.SendCommand(GetEvalCommand(cmd, allowTopLevelAwait), timeoutseconds = 30);
        }

        public string SendCommand(string cmd, uint timeoutseconds = 30)
        {

            string ep;
            lock (Chrome.padLock)
            {
                ep = this.sessionWSEndpoint;
            }
            string message = "";
            ManualResetEvent waitEvent = new ManualResetEvent(false);
            Exception exc = null;
            bool active = true;

            var t = new Thread(new ThreadStart(() =>
            {
                try
                {

                    WebSocket4Net.WebSocket j = new WebSocket4Net.WebSocket(ep);
                    j.LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
                    byte[] data;

                    j.Opened += delegate (System.Object o, EventArgs e)
                    {
                        j.Send(cmd);
                    };

                    j.MessageReceived += delegate (System.Object o, MessageReceivedEventArgs e)
                    {
                        message = e.Message;
                        active = false;
                    };

                    j.Error += delegate (System.Object o, SuperSocket.ClientEngine.ErrorEventArgs e)
                    {
                        active = false;
                        exc = e.Exception;
                    };

                    j.Closed += delegate (System.Object o, EventArgs e)
                    {
                        active = false;
                    };

                    j.DataReceived += delegate (System.Object o, DataReceivedEventArgs e)
                    {
                        active = false;
                        data = e.Data;
                    };

                    j.Open();
                    System.DateTime startTime = System.DateTime.UtcNow;

                    while (active && (System.DateTime.UtcNow - startTime).TotalSeconds < timeoutseconds && Thread.CurrentThread.ThreadState == ThreadState.Running)
                    {
                        Thread.Sleep(10);
                    }

                    try
                    {
                        if (j.State != WebSocket4Net.WebSocketState.Closed)
                        {
                            j.Close();
                        }
                    }
                    catch { }
                }
                finally
                {
                    waitEvent.Set();
                }
            }));

            t.Start();

            waitEvent.WaitOne();

            if (exc != null) throw exc;

            return message;
        }

        private T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        private T Deserialise<T>(Stream json)
        {
            T obj = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(json);
            return obj;
        }

        public void SetActiveSession(string sessionWSEndpoint)
        {
            lock (padLock)
                // Sometimes binding to localhost might resolve wrong AddressFamily, force IPv4
                this.sessionWSEndpoint = sessionWSEndpoint.Replace("ws://localhost", "ws://127.0.0.1").Replace("wss://localhost", "wss://127.0.0.1");
        }
    }
}
