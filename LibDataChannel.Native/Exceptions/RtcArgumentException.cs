namespace LibDataChannel.Native.Exceptions;

internal class RtcArgumentException : RtcException
{
    public RtcArgumentException() : base("Invalid argument provided.")
    {
    }
}