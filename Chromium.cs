using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.Serialization.Json;
using System.Text;
using Websocket.Client;

namespace GenXdev.Helpers
{
    public class Chromium
    {
        public static Int64 Id = 0;
        public static object padLock = new object();

        const string JsonPostfix = "/json";

        public string remoteDebuggingUri;
        public string sessionWSEndpoint;

        public int Port { get; private set; }
        public Chromium(string remoteDebuggingUri)
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
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":" + Serialization.ToJson("document.location='" + uri + "'") + @",""objectGroup"":""console"",""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":" + (Interlocked.Increment(ref Chromium.Id)) + "}";
            return this.SendCommand(json);
        }

        public string GetElementsByTagName(string tagName)
        {
            // Page.navigate is working from M18
            //var json = @"{""method"":""Page.navigate"",""params"":{""url"":""http://www.seznam.cz""},""id"":1}";

            // Instead of Page.navigate, we can use document.location
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""document.getElementsByTagName('" + tagName + @"')"",""objectGroup"":""console"",""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":" + (Interlocked.Increment(ref Chromium.Id)) + "}";
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
            // /* @"""objectGroup"":""console"",*/ 
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":" + Serialization.ToJson(cmd) + @"," + awaitParam + @"""allowUnsafeEvalBlockedByCSP"":true,""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":true,""returnByValue"":true,""awaitPromise"":true},""id"":" + (Interlocked.Increment(ref Chromium.Id)) + "}";

            // return json;
            return json;
        }

        public string Eval(string cmd, bool allowTopLevelAwait = false, uint timeoutseconds = 30)
        {
            // return json;
            return this.SendCommand(GetEvalCommand(cmd, allowTopLevelAwait), timeoutseconds);
        }

        public static ConcurrentDictionary<string, WebsocketClient> SocketCache = new ConcurrentDictionary<string, WebsocketClient>();
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> SocketCacheData = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public string SendCommand(string cmd, uint timeoutseconds = 30)
        {
            // // Console.WriteLine("SendCommand: timeout: " + timeoutseconds.ToString() + " seconds");

            var id = Interlocked.Read(ref Chromium.Id);
            var url = new Uri(this.sessionWSEndpoint);
            var padlock = new object();
            System.DateTime lastRx = System.DateTime.UtcNow;

            WebsocketClient client = null;
            if (SocketCache.ContainsKey(url.ToString()))
            {
                client = SocketCache[url.ToString()];
                if (!client.IsStarted)
                {

                    client.Start();
                }
            }
            else
            {
                var factory = new Func<ClientWebSocket>(() => new ClientWebSocket
                {
                    Options = {

                        KeepAliveInterval = TimeSpan.FromSeconds(0.25)
                }
                });

                client = new WebsocketClient(url, factory);
                client.DisconnectionHappened.Subscribe(info =>
                {
                    if (SocketCacheData.ContainsKey(url.ToString())) {

                        ConcurrentQueue<string> queue = null;
                        SocketCacheData.TryRemove(url.ToString(), out queue);
                    }// Console.WriteLine($"Disconnected: {info.Type}");
                });

                client.MessageReceived.Subscribe(msg =>
                {
                    if (msg == null)
                    {
                        // // Console.WriteLine("received: NULL");
                        return;
                    }

                    ConcurrentQueue<string> queue = null;

                    if (SocketCacheData.ContainsKey(url.ToString()))
                    {
                        queue = SocketCacheData[url.ToString()];
                    }
                    else
                    {
                        queue = new ConcurrentQueue<string>();
                        SocketCacheData[url.ToString()] = queue;
                    }

                    queue.Enqueue(msg.Text);

                });

                client.Start();
                SocketCache[url.ToString()] = client;
            }

            // Console.WriteLine("sending: " + cmd);
            Task.Run(() => client.Send(cmd));
            bool hadData = false;
            while (!Console.KeyAvailable)
            {
                ConcurrentQueue<string> queue = null;
                if (SocketCacheData.ContainsKey(url.ToString()))
                {
                    queue = SocketCacheData[url.ToString()];
                    hadData = true;
                }
                else
                {
                    if (hadData)
                    {
                        break;
                    }
                    queue = new ConcurrentQueue<string>();
                    SocketCacheData[url.ToString()] = queue;
                }

                string msg = null;
                if (queue.TryDequeue(out msg))
                {
                    if (msg.StartsWith("{\"id\":" + id.ToString()))
                    {
                        // Console.WriteLine("returned: " + msg);
                        return msg;
                    }

                    // Console.WriteLine("received: " + msg);
                    queue.Enqueue(msg);                    
                }

                if ((System.DateTime.UtcNow - lastRx).TotalSeconds > timeoutseconds)
                {
                    return null;
                }

                Thread.Sleep(10);
            }

            return null;
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
