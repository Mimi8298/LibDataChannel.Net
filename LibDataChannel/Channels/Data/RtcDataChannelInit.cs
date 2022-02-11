namespace LibDataChannel.Channels.Data;

using System.Runtime.InteropServices;
using LibDataChannel.Native.Channels.Data;

/// <summary>
///     Represents the data channel initialization parameters.
/// </summary>
public class RtcDataChannelInit
{
    private ushort? _streamId;
    
    /// <summary>
    ///     The reliability of the channel.
    /// </summary>
    public RtcReliability Reliability { get; set; }
    
    /// <summary>
    ///     The sub-protocol used for the channel.
    /// </summary>
    public string Protocol { get; set; }
    
    /// <summary>
    ///     True if the channel is assumed to be negotiated by the user and won't be negotiated by the WebRTC layer.
    /// </summary>
    public bool Negotiated { get; set; }

    /// <summary>
    ///     The stream id to use for the channel. It musts be in the [0, 65534] range.
    /// </summary>
    public ushort? StreamId
    {
        get => _streamId;
        set
        {
            if (value is < 0 or > 65534)
                throw new ArgumentException("StreamId must be in the [0, 65534] range.");
            
            _streamId = value;
        }
    }

    internal NativeRtcDataChannelInit AllocNative() => new NativeRtcDataChannelInit
    {
        Reliability = Reliability != null ? Reliability.AllocNative() : default,
        Protocol = Marshal.StringToHGlobalAnsi(Protocol),
        Negotiated = Negotiated,
        ManualStream = StreamId.HasValue,
        StreamId = StreamId ?? 0
    };
}