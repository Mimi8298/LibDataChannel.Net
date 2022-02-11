namespace LibDataChannel.Channels.Data;

using System.Diagnostics;
using LibDataChannel.Connections.Rtc;
using LibDataChannel.Native.Channels.Data;

[DebuggerDisplay("RtcDataChannel({_label})")]
public class RtcDataChannel : RtcChannel
{
    /// <summary>
    ///     THe parent peer connection.
    /// </summary>
    public RtcPeerConnection PeerConnection { get; }
    
    private string _label;
    private string _protocol;
    private RtcReliability _reliability;
    
    internal RtcDataChannel(RtcPeerConnection peerConnection, string label) 
        : base(NativeRtcDataChannel.Create(peerConnection, label))
    {
        PeerConnection = peerConnection;
        PeerConnection.OnDataChannelAdded(this);
    }
    
    internal RtcDataChannel(RtcPeerConnection peerConnection, string label, RtcDataChannelInit init) 
        : base(NativeRtcDataChannel.Create(peerConnection, label, init.AllocNative()))
    {
        PeerConnection = peerConnection;
        PeerConnection.OnDataChannelAdded(this);
    }

    internal RtcDataChannel(RtcPeerConnection peerConnection, int id) : base(id)
    {
        PeerConnection = peerConnection;
        PeerConnection.OnDataChannelAdded(this);
    }

    protected override void OnDispose()
    {
        PeerConnection.OnDataChannelClosed(this);
        base.OnDispose();
    }

    protected override void Free()
    {
        NativeRtcDataChannel.Delete(this);
    }

    /// <summary>
    ///     The label of the data channel.
    /// </summary>
    public string Label => _label ??= NativeRtcDataChannel.GetLabel(this);
    
    /// <summary>
    ///     The protocol of the data channel.
    /// </summary>
    public string Protocol => _protocol ??= NativeRtcDataChannel.GetProtocol(this);
    
    /// <summary>
    ///     The reliability of the data channel.
    /// </summary>
    public RtcReliability Reliability => _reliability ??= RtcReliability.FromNative(NativeRtcDataChannel.GetReliability(this));
}