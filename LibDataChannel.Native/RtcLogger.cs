namespace LibDataChannel.Native;

public abstract class RtcLogger : IDisposable
{
    public RtcLogLevel Level { get; }

    protected RtcLogger(RtcLogLevel level)
    {
        Level = level;
        NativeRtcLogger.Attach(this);
    }
    
    public void Dispose()
    {
        NativeRtcLogger.Detach(this);
    }

    public abstract void Log(RtcLogLevel level, string message);
}