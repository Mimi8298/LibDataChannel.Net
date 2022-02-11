namespace LibDataChannel.Native.Connections.Rtc;

using System.Runtime.InteropServices;
using LibDataChannel.Native.Sdp;

/// <summary>
///     Handle for a peer connection.
/// </summary>
public abstract class NativeRtcPeerConnectionHandle : NativeRtcHandle
{
    /// <summary>
    ///     Id of the peer connection.
    /// </summary>
    public int Id { get; }

    protected NativeRtcPeerConnectionHandle(int id)
    {
        Id = id;
        
        NativeRtcPeerConnection.SetHandle(this, HandlePtr);
        NativeRtcPeerConnection.AttachCallbacks(this);
    }

    protected override void OnDispose()
    {
    }

    protected override void Free()
    {
        NativeRtcPeerConnection.Delete(this);
    }

    protected internal abstract void Internal_OnLocalDescription(SdpMessage description);
    protected internal abstract void Internal_OnLocalCandidate(RtcIceCandidate candidate);
    protected internal abstract void Internal_StateChangeCallback(RtcState state);
    protected internal abstract void Internal_GatheringStateCallback(RtcGatheringState state);
    protected internal abstract void Internal_SignalingStateCallback(RtcSignalingState state);
    protected internal abstract void Internal_DataChannelCallback(int dataChannelId);
    
    public static NativeRtcPeerConnectionHandle FromHandle(IntPtr handle) => (NativeRtcPeerConnectionHandle) GCHandle.FromIntPtr(handle).Target;
}