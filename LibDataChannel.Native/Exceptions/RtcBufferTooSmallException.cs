namespace LibDataChannel.Native.Exceptions;

internal class RtcBufferTooSmallException : RtcException
{
    public RtcBufferTooSmallException() : base("Buffer too small")
    {
    }
}