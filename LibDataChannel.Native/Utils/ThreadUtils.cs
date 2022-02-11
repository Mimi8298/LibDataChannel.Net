namespace LibDataChannel.Native.Utils;

public static class ThreadUtils
{
    [ThreadStatic] private static bool _isRtcThread;
    
    public static bool IsRtcThread() => _isRtcThread;
    public static void SetRtcThread() => _isRtcThread = true;
}