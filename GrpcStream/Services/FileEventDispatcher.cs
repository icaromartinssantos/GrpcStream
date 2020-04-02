namespace GrpcStream.Services
{
    public class FileEventDispatcher
    {
        public delegate void contentSent(byte[] msg);
        public event contentSent ContentSent;

        public void OnSentEvent(byte[] cont){
            if (ContentSent != null)
                ContentSent(cont);
        }
    }
}