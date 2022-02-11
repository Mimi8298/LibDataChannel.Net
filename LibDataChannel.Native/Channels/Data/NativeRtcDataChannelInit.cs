namespace LibDataChannel.Native.Channels.Data;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public ref struct NativeRtcDataChannelInit
{
    public NativeRtcReliability Reliability;
    public IntPtr Protocol;
    public bool Negotiated;
    public bool ManualStream;
    public ushort StreamId;

    public NativeRtcDataChannelInit(in NativeRtcReliability reliability, IntPtr protocol, bool negotiated, bool manualStream, ushort streamId)
    {
        Reliability = reliability;
        Protocol = protocol;
        Negotiated = negotiated;
        ManualStream = manualStream;
        StreamId = streamId;
    }

    public void Free()
    {
        Marshal.FreeHGlobal(Protocol);
    }
}