namespace LibDataChannel.Native.Exceptions;

internal class RtcFailureException : RtcException
{
    public RtcFailureException() : base("Rtc operation failed")
    {
    }
}