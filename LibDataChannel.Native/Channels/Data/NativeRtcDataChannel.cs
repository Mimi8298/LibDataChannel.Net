namespace LibDataChannel.Native.Channels.Data;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LibDataChannel.Native.Connections.Rtc;

public static class NativeRtcDataChannel
{
    private const int StringBufferSize = 8192;
    
    /// <summary>
    ///     Creates a new native RTCDataChannel object with the given label and default options.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="label">the label.</param>
    /// <returns>the data channel id.</returns>
    public static unsafe int Create(NativeRtcPeerConnectionHandle handle, string label)
    {
        ArgumentNullException.ThrowIfNull(label, nameof(label));
        Span<byte> utf8Label = stackalloc byte[Encoding.UTF8.GetByteCount(label) + 1];
        Encoding.UTF8.GetBytes(label, utf8Label);
        utf8Label[^1] = 0;

        return NativeRtc.CreateDataChannel(handle.Id, (IntPtr) Unsafe.AsPointer(ref utf8Label.GetPinnableReference()));
    }
    
    /// <summary>
    ///     Creates a new native RTCDataChannel object with the given label and default options.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="label">the label.</param>
    /// <param name="init">the options.</param>
    /// <returns>the data channel id.</returns>
    public static unsafe int Create(NativeRtcPeerConnectionHandle handle, string label, NativeRtcDataChannelInit init)
    {
        ArgumentNullException.ThrowIfNull(label, nameof(label));
        Span<byte> utf8Label = stackalloc byte[Encoding.UTF8.GetByteCount(label) + 1];
        Encoding.UTF8.GetBytes(label, utf8Label);
        utf8Label[^1] = 0;

        return NativeRtc.CreateDataChannel(handle.Id, (IntPtr) Unsafe.AsPointer(ref utf8Label.GetPinnableReference()), (IntPtr) (&init));
    }

    /// <summary>
    ///     Deletes the native RTCDataChannel object.
    /// </summary>
    /// <param name="handle">the handle.</param>
    public static void Delete(NativeRtcChannelHandle handle)
    {
        NativeRtc.DeleteDataChannel(handle.Id);
    }
    
    /// <summary>
    ///     Gets the stream id.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the stream id.</returns>
    public static int GetStream(NativeRtcChannelHandle handle)
    {
        return NativeRtc.GetDataChannelStream(handle.Id);
    }

    /// <summary>
    ///     Gets the channel label.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the label.</returns>
    public static unsafe string GetLabel(NativeRtcChannelHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int length = NativeRtc.GetDataChannelLabel(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        return Marshal.PtrToStringAnsi((IntPtr) buffer, length - 1);
    }
    
    /// <summary>
    ///     Gets the channel sub-protocol.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the sub-protocol name.</returns>
    public static unsafe string GetProtocol(NativeRtcChannelHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int length = NativeRtc.GetDataChannelProtocol(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        return Marshal.PtrToStringAnsi((IntPtr) buffer, length - 1);
    }

    /// <summary>
    ///     Gets the reliability of the channel.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the reliability options.</returns>
    public static unsafe NativeRtcReliability GetReliability(NativeRtcChannelHandle handle)
    {
        NativeRtcReliability reliability;
        int errorCode = NativeRtc.GetDataChannelReliability(handle.Id, (IntPtr) (&reliability));
        
        if (errorCode != 0)
            NativeRtc.ThrowException(errorCode);

        return reliability;
    }
}