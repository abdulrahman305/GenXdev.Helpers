namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleServerAuthenticationEventArgs : EventArgs
    {
        public byte[] Authentication { get; set; }
        public bool Handled { get; set; }
    }
}
