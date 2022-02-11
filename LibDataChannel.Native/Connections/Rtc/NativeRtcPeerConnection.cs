namespace LibDataChannel.Native.Connections.Rtc;

using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LibDataChannel.Native.Exceptions;
using LibDataChannel.Native.Sdp;
using LibDataChannel.Native.Utils;

/// <summary>
///     Represents the list of functions used by the <see cref="NativeRtcPeerConnectionHandle"/>.
/// </summary>
public static class NativeRtcPeerConnection
{
    private const int StringBufferSize = 65535;
        
    private static readonly IntPtr[] _descriptionTypeAnsiStringPointers =
    {
        Marshal.StringToHGlobalAnsi("offer"),
        Marshal.StringToHGlobalAnsi("answer"),
        Marshal.StringToHGlobalAnsi("pranswer"),
        Marshal.StringToHGlobalAnsi("rollback"),
    };

    private static readonly byte[][] _descriptionTypeAnsiStringBytes =
    {
        new[] {(byte) 'o', (byte) 'f', (byte) 'f', (byte) 'e', (byte) 'r', (byte) '\0'},
        new[] {(byte) 'a', (byte) 'n', (byte) 's', (byte) 'w', (byte) 'e', (byte) 'r', (byte) '\0'},
        new[] {(byte) 'p', (byte) 'r', (byte) 'a', (byte) 'n', (byte) 's', (byte) 'w', (byte) 'e', (byte) 'r', (byte) '\0'},
        new[] {(byte) 'r', (byte) 'o', (byte) 'l', (byte) 'l', (byte) 'b', (byte) 'a', (byte) 'c', (byte) 'k', (byte) '\0'}
    };
    
    /// <summary>
    ///     Creates a new peer connection in the native implementation.
    /// </summary>
    /// <param name="configuration">the configuration to pass.</param>
    /// <returns>The peer connection id.</returns>
    public static unsafe int Create(NativeRtcConfiguration configuration)
    {
        try
        {
            return NativeRtc.CreatePeerConnection((IntPtr) (&configuration));
        }
        finally
        {
            configuration.Free();
        }
    }
    
    /// <summary>
    ///     Deletes the peer connection from the native implementation.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    public static void Delete(NativeRtcPeerConnectionHandle handle) => NativeRtc.DeletePeerConnection(handle.Id);

    /// <summary>
    ///     Sets the user pointer to the peer connection.
    /// </summary>
    /// <param name="managedHandle">the managed handle.</param>
    /// <param name="handle">the pinned handle pointer.</param>
    public static void SetHandle(NativeRtcPeerConnectionHandle managedHandle, IntPtr handle) => NativeRtc.SetUserPointer(managedHandle.Id, handle);

    /// <summary>
    ///     Sets the local description to the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <param name="type">the description type.</param>
    public static void SetLocalDescription(NativeRtcPeerConnectionHandle handle, SdpType type)
    {
        int errorCode = NativeRtc.SetLocalDescription(handle.Id, _descriptionTypeAnsiStringPointers[(int) type]);
        
        if (errorCode != 0)
        {
            NativeRtc.ThrowException(errorCode);
        }
    }
    
    /// <summary>
    ///     Sets the remote description to the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <param name="sdp">the sdp description message.</param>
    public static void SetRemoteDescription(NativeRtcPeerConnectionHandle handle, SdpMessage sdp)
    {
        ArgumentNullException.ThrowIfNull(sdp.Content, nameof(sdp.Content));
        IntPtr stringToHGlobalAnsi = Marshal.StringToHGlobalAnsi(sdp.Content);
        
        try
        {
            int errorCode = NativeRtc.SetRemoteDescription(handle.Id, stringToHGlobalAnsi, _descriptionTypeAnsiStringPointers[(int) sdp.Type]);

            if (errorCode != 0)
            {
                NativeRtc.ThrowException(errorCode);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(stringToHGlobalAnsi);
        }
    }
    
    /// <summary>
    ///     Adds a remote ice candidate to the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <param name="candidate">the ice candidate.</param>
    public static void AddRemoteCandidate(NativeRtcPeerConnectionHandle handle, RtcIceCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate.Candidate, nameof(candidate));
        
        IntPtr candidatePtr = Marshal.StringToHGlobalAnsi(candidate.Candidate);
        IntPtr midPtr = Marshal.StringToHGlobalAnsi(candidate.Mid);

        try
        {
            int errorCode = NativeRtc.AddRemoteCandidate(handle.Id, candidatePtr, midPtr);
            
            if (errorCode != 0)
            {
                NativeRtc.ThrowException(errorCode);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(candidatePtr);
            Marshal.FreeHGlobal(midPtr);
        }
    }
    
    /// <summary>
    ///     Gets the local description from the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>The local description (sdp content).</returns>
    public static unsafe string GetLocalDescription(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetLocalDescription(handle.Id, (IntPtr) buffer, StringBufferSize);

        if (size <= 0)
            NativeRtc.ThrowException(size);
        
        return Marshal.PtrToStringAnsi((IntPtr) buffer, size - 1);
    }
    
    /// <summary>
    ///     Gets the remote description from the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>The remote description (sdp content).</returns>
    public static unsafe string GetRemoteDescription(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetRemoteDescription(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        if (size <= 0)
            NativeRtc.ThrowException(size);
        
        return Marshal.PtrToStringAnsi((IntPtr) buffer, size - 1);
    }
    
    /// <summary>
    ///     Gets the local description type from the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>The local description type (sdp type).</returns>
    public static unsafe SdpType GetLocalDescriptionType(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetLocalDescriptionType(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        if (size <= 0)
            NativeRtc.ThrowException(size);

        return GetDescriptionTypeFromAnsi(MemoryMarshal.CreateReadOnlySpan(ref buffer[0], size));
    }
    
    /// <summary>
    ///     Gets the remote description type from the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>The remote description type (sdp type).</returns>
    public static unsafe SdpType GetRemoteDescriptionType(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetRemoteDescriptionType(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        if (size <= 0)
            NativeRtc.ThrowException(size);

        return GetDescriptionTypeFromAnsi(MemoryMarshal.CreateReadOnlySpan(ref buffer[0], size));
    }
    
    /// <summary>
    ///     Gets the local ip endpoint of the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>the ip endpoint.</returns>
    public static unsafe IPEndPoint GetLocalAddress(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetLocalAddress(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        if (size <= 0)
            NativeRtc.ThrowException(size);
        
        Span<char> charBuffer = stackalloc char[StringBufferSize];
        Span<char> ipString = charBuffer[..Encoding.ASCII.GetChars(new ReadOnlySpan<byte>(buffer, size - 1), charBuffer)];

        return IPEndPoint.Parse(ipString);
    }
    
    /// <summary>
    ///     Gets the remote ip endpoint of the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns>the ip endpoint.</returns>
    public static unsafe IPEndPoint GetRemoteAddress(NativeRtcPeerConnectionHandle handle)
    {
        byte *buffer = stackalloc byte[StringBufferSize];
        int size = NativeRtc.GetRemoteAddress(handle.Id, (IntPtr) buffer, StringBufferSize);
        
        if (size <= 0)
            NativeRtc.ThrowException(size);
        
        Span<char> charBuffer = stackalloc char[StringBufferSize];
        Span<char> ipString = charBuffer[..Encoding.ASCII.GetChars(new ReadOnlySpan<byte>(buffer, size - 1), charBuffer)];

        return IPEndPoint.Parse(ipString);
    }

    /// <summary>
    ///     Gets the selected candidate pair of the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <param name="local">the local candidate.</param>
    /// <param name="remote">the remote candidate.</param>
    public static unsafe void GetSelectedCandidatePair(NativeRtcPeerConnectionHandle handle, out string local, out string remote)
    {
        byte* localBuffer = stackalloc byte[StringBufferSize];
        byte* remoteBuffer = stackalloc byte[StringBufferSize];
        int maxSize = NativeRtc.GetSelectedCandidatePair(handle.Id, (IntPtr) localBuffer, StringBufferSize, (IntPtr) remoteBuffer, StringBufferSize);
        
        if (maxSize <= 0)
            NativeRtc.ThrowException(maxSize);
        
        local = Marshal.PtrToStringAnsi((IntPtr) localBuffer, maxSize - 1);
        remote = Marshal.PtrToStringAnsi((IntPtr) remoteBuffer, maxSize - 1);
    }

    #region Callback
    
    /// <summary>
    ///     Registers managed callbacks to the peer connection.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    public static unsafe void AttachCallbacks(NativeRtcPeerConnectionHandle handle)
    {
        int id = handle.Id;
        NativeRtc.SetLocalDescription(id, &RtcDescriptionCallback);
        NativeRtc.SetLocalCandidateCallback(id, &LocalCandidateCallback);
        NativeRtc.SetStateChangeCallback(id, &StateChangeCallback);
        NativeRtc.SetGatheringStateChangeCallback(id, &GatheringStateCallback);
        NativeRtc.SetSignalingStateChangeCallback(id, &SignalingStateCallback);
        NativeRtc.SetDataChannelCallback(id, &DataChannelCallback);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void RtcDescriptionCallback(int id, IntPtr sdp, IntPtr type, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        SdpMessage sessionDescription = new SdpMessage(Enum.Parse<SdpType>(Marshal.PtrToStringAnsi(type)!, true), Marshal.PtrToStringAnsi(sdp));
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_OnLocalDescription(sessionDescription);
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void LocalCandidateCallback(int id, IntPtr candidate, IntPtr mid, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        RtcIceCandidate iceCandidate = new RtcIceCandidate(Marshal.PtrToStringAnsi(candidate), Marshal.PtrToStringAnsi(mid));
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_OnLocalCandidate(iceCandidate);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void StateChangeCallback(int id, RtcState state, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_StateChangeCallback(state);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void GatheringStateCallback(int id, RtcGatheringState state, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_GatheringStateCallback(state);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void SignalingStateCallback(int id, RtcSignalingState state, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_SignalingStateCallback(state);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void DataChannelCallback(int id, int dataChannelId, IntPtr userPointer)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcPeerConnectionHandle.FromHandle(userPointer).Internal_DataChannelCallback(dataChannelId);
    }
    
    #endregion
    
    /// <summary>
    ///     Finds the description type from a ansi string.
    /// </summary>
    /// <param name="ansi">the ansi string.</param>
    /// <returns>the sdp type</returns>
    private static SdpType GetDescriptionTypeFromAnsi(ReadOnlySpan<byte> ansi)
    {
        for (int i = 0; i < _descriptionTypeAnsiStringBytes.Length; i++)
        {
            if (ansi.SequenceEqual(_descriptionTypeAnsiStringBytes[i]))
            {
                return (SdpType) i;
            }
        }
        
        throw new RtcException($"Unknown description type: {Encoding.ASCII.GetString(ansi)}");
    }
}