namespace GenXdev.AsyncSockets.Arguments
{
    public class AuthenticateClientEventArgs : EventArgs
    {
        public byte[] ClientAuthentication { get; internal set; }
        public bool Result { get; set; }
        public bool Handled { get; set; }
    }
}
