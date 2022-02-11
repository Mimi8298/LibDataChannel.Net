namespace LibDataChannel.Native.Exceptions;

internal class RtcNotAvailableException : RtcException
{
    public RtcNotAvailableException() : base("Data not available in the current state.")
    {
    }
}