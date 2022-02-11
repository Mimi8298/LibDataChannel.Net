namespace LibDataChannel.Native.Channels;

using System.Runtime.InteropServices;

/// <summary>
///     Handle for a data channel.
/// </summary>
public abstract class NativeRtcChannelHandle : NativeRtcHandle
{
    /// <summary>
    ///     The channel id.
    /// </summary>
    public int Id { get; }

    protected NativeRtcChannelHandle(int id)
    {
        Id = id;
        
        NativeRtcChannel.SetHandle(this, HandlePtr);
        NativeRtcChannel.AttachCallbacks(this);
    }

    protected override void OnDispose()
    {
    }

    protected internal abstract void Internal_OnOpen();
    protected internal abstract void Internal_OnClosed();
    protected internal abstract void Internal_OnError(string error);
    protected internal abstract void Internal_OnMessage(ReadOnlySpan<byte> message);
    protected internal abstract void Internal_OnBufferedAmountLow();
    protected internal abstract void Internal_OnAvailable();

    public static NativeRtcChannelHandle FromHandle(IntPtr ptr) => (NativeRtcChannelHandle) GCHandle.FromIntPtr(ptr).Target;
}