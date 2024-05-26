using System.Runtime.Serialization;

namespace GenXdev.Helpers
{
    public class RemoteSessions : IChromeRequest
    {
                
    }
    
    [Serializable]
    [DataContract]
    public class RemoteSessionsResponse
    {
        public RemoteSessionsResponse() { }

        [DataMember]
        public string devtoolsFrontendUrl;

        [DataMember]
        public string faviconUrl;

        [DataMember]
        public string thumbnailUrl;

        [DataMember]
        public string title;
        
        [DataMember]
        public string url;

        [DataMember]
        public string webSocketDebuggerUrl;
    }
}
