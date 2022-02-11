namespace LibDataChannel.Native.Channels.Data;

using System.Runtime.InteropServices;

/// <summary>
///     Native structure mapping for the rtc reliability configuration.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public ref struct NativeRtcReliability
{
    public bool Unordered;
    public bool Unreliable;
    public uint MaxPacketLifeTime;
    public uint MaxRetransmits;
}