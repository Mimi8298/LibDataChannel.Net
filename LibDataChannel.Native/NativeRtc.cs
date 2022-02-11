namespace LibDataChannel.Native;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibDataChannel.Native.Channels.Data;
using LibDataChannel.Native.Connections.Rtc;
using LibDataChannel.Native.Exceptions;

public unsafe class NativeRtc
{
    private const string DllName = "datachannel";

    internal const int ErrorInvalidArgument = -1;
    internal const int ErrorFailure = -2;
    internal const int ErrorNotAvailable = -3;
    internal const int ErrorBufferTooSmall = -4;

    [DllImport(DllName, EntryPoint = "rtcInitLogger", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitLogger(RtcLogLevel level, delegate* unmanaged[Cdecl]<RtcLogLevel, IntPtr, void> callback);

    [DllImport(DllName, EntryPoint = "rtcPreload", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Preload();

    [DllImport(DllName, EntryPoint = "rtcCleanup", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Cleanup();

    [DllImport(DllName, EntryPoint = "rtcSetUserPointer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUserPointer(int id, IntPtr userPointer);
    
    [DllImport(DllName, EntryPoint = "rtcCreatePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CreatePeerConnection(IntPtr configuration);
    
    [DllImport(DllName, EntryPoint = "rtcDeletePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeletePeerConnection(int peerConnectionId);

    [DllImport(DllName, EntryPoint = "rtcSetLocalDescriptionCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalDescription(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetLocalCandidateCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalCandidateCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcState, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetGatheringStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetGatheringStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcGatheringState, IntPtr, void> callback);

    [DllImport(DllName, EntryPoint = "rtcSetSignalingStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetSignalingStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcSignalingState, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetDataChannelCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetDataChannelCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetLocalDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalDescription(int peerConnectionId, IntPtr type);
    
    [DllImport(DllName, EntryPoint = "rtcSetRemoteDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetRemoteDescription(int peerConnectionId, IntPtr sdp, IntPtr type);
    
    [DllImport(DllName, EntryPoint = "rtcAddRemoteCandidate", CallingConvention = CallingConvention.Cdecl)]
    public static extern int AddRemoteCandidate(int peerConnectionId, IntPtr candidate, IntPtr mid);
    
    [DllImport(DllName, EntryPoint = "rtcGetLocalDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalDescription(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetRemoteDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteDescription(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetLocalDescriptionType", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalDescriptionType(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetRemoteDescriptionType", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteDescriptionType(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetLocalAddress", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalAddress(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetRemoteAddress", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteAddress(int peerConnectionId, IntPtr buffer, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetSelectedCandidatePair", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetSelectedCandidatePair(int peerConnectionId, IntPtr local, int localSize, IntPtr remote, int remoteSize);
    
    [DllImport(DllName, EntryPoint = "rtcSetOpenCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetOpenCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetClosedCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetClosedCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetErrorCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetErrorCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetMessageCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetMessageCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetBufferedAmountLowCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcSetAvailableCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetAvailableCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(DllName, EntryPoint = "rtcIsOpen", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsOpen(int channelId);
    
    [DllImport(DllName, EntryPoint = "rtcIsClosed", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsClosed(int channelId);
    
    [DllImport(DllName, EntryPoint = "rtcSendMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SendMessage(int channelId, IntPtr message, int size);
    
    [DllImport(DllName, EntryPoint = "rtcGetBufferedAmount", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetBufferedAmount(int channelId);
    
    [DllImport(DllName, EntryPoint = "rtcSetBufferedAmountLowThreshold", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowThreshold(int channelId, int amount);
    
    [DllImport(DllName, EntryPoint = "rtcReceiveMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ReceiveMessage(int channelId, IntPtr buffer, IntPtr pSize);
    
	[DllImport(DllName, EntryPoint = "rtcGetAvailableAmount", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetAvailableAmount(int channelId);
	
	[DllImport(DllName, EntryPoint = "rtcCreateDataChannel", CallingConvention = CallingConvention.Cdecl)]
	public static extern int CreateDataChannel(int peerConnectionId, IntPtr label);
	
	[DllImport(DllName, EntryPoint = "rtcCreateDataChannelEx", CallingConvention = CallingConvention.Cdecl)]
	public static extern int CreateDataChannel(int peerConnectionId, IntPtr label, IntPtr init);
	
	[DllImport(DllName, EntryPoint = "rtcDeleteDataChannel", CallingConvention = CallingConvention.Cdecl)]
	public static extern int DeleteDataChannel(int dataChannelId);
	
	[DllImport(DllName, EntryPoint = "rtcGetDataChannelStream", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetDataChannelStream(int dataChannelId);
	
	[DllImport(DllName, EntryPoint = "rtcGetDataChannelLabel", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetDataChannelLabel(int dataChannelId, IntPtr buffer, int size);
	
	[DllImport(DllName, EntryPoint = "rtcGetDataChannelProtocol", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetDataChannelProtocol(int dataChannelId, IntPtr buffer, int size);
	
	[DllImport(DllName, EntryPoint = "rtcGetDataChannelReliability", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetDataChannelReliability(int dataChannelId, IntPtr reliability);
	
	// TODO: TRACK & MEDIA & WEBSOCKET
	
	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowException(int errorCode)
	{
		throw errorCode switch
		{
			ErrorInvalidArgument => new RtcArgumentException(),
			ErrorFailure => new RtcFailureException(),
			ErrorNotAvailable => new RtcNotAvailableException(),
			ErrorBufferTooSmall => new RtcBufferTooSmallException(),
			_ => new RtcException($"RTC error code: {errorCode}"),
		};
	}
}