namespace LibDataChannel.Channels;

using LibDataChannel.Native.Channels.Data;

/// <summary>
///     Represents a data channel reliability configuration.
/// </summary>
public class RtcReliability
{
    /// <summary>
    ///     True if the data channel must be send message in order. True by default.
    /// </summary>
    public bool Ordered { get; set; } = true;
    
    /// <summary>
    ///     True if the data channel must retransmit lost messages. True by default.
    /// </summary>
    public bool Reliable { get; set; } = true;
    
    /// <summary>
    ///     If unreliable, maximum packet life time in milliseconds
    /// </summary>
    public uint MaxPacketLifeTime { get; set; }
    
    /// <summary>
    ///     If unreliable and <see cref="MaxPacketLifeTime"/> is 0, maximum number of retransmissions (0 means no retransmission).
    /// </summary>
    public uint MaxRetransmits { get; set; }

    internal NativeRtcReliability AllocNative() => new NativeRtcReliability
    {
        Unordered = !Ordered,
        Unreliable = !Reliable,
        MaxPacketLifeTime = MaxPacketLifeTime,
        MaxRetransmits = MaxRetransmits
    };

    internal static RtcReliability FromNative(NativeRtcReliability native)
    {
        return new RtcReliability
        {
            Ordered = !native.Unordered,
            Reliable = !native.Unreliable,
            MaxPacketLifeTime = native.MaxPacketLifeTime,
            MaxRetransmits = native.MaxRetransmits
        };
    }
}