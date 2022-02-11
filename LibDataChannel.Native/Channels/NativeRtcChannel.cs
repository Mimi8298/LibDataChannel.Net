namespace LibDataChannel.Native.Channels;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibDataChannel.Native.Utils;

/// <summary>
///     Represents the list of functions used by <see cref="NativeRtcChannelHandle"/>.
/// </summary>
public static class NativeRtcChannel
{
    /// <summary>
    ///     Sets the user pointer to the channel.
    /// </summary>
    /// <param name="managedHandle">the managed handle.</param>
    /// <param name="handle">the pinned handle pointer.</param>
    public static void SetHandle(NativeRtcChannelHandle managedHandle, IntPtr handle) => NativeRtc.SetUserPointer(managedHandle.Id, handle);
    
    /// <summary>
    ///     Gets whether the channel is open.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <returns>true if open.</returns>
    public static bool IsOpen(NativeRtcChannelHandle handle) => NativeRtc.IsOpen(handle.Id);
    
    /// <summary>
    ///     Gets whether the channel is closed.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <returns>true if closed.</returns>
    public static bool IsClosed(NativeRtcChannelHandle handle) => NativeRtc.IsClosed(handle.Id);

    /// <summary>
    ///     Sends the given message data.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <param name="message">the message content.</param>
    /// <returns>true if closed.</returns>
    public static unsafe int SendMessage(NativeRtcChannelHandle handle, ReadOnlySpan<byte> message)
    {
        fixed(byte *data = message)
        {
            return NativeRtc.SendMessage(handle.Id, (IntPtr) data, message.Length);
        }
    }
    
    /// <summary>
    ///     Gets the total size of buffered messages waiting to be sent.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <returns></returns>
    public static int GetBufferedAmount(NativeRtcChannelHandle handle) => NativeRtc.GetBufferedAmount(handle.Id);
    
    /// <summary>
    ///     Sets the buffered amount threshold under which BufferedAmountLowCallback is called.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <param name="threshold">the buffer level threshold.</param>
    public static void SetBufferedAmountLowThreshold(NativeRtcChannelHandle handle, int threshold) => NativeRtc.SetBufferedAmountLowThreshold(handle.Id, threshold);

    /// <summary>
    ///     Tries to receive a message from the channel.
    ///     Must be called only if there is no message callback registered.
    /// </summary>
    /// <param name="handle">the channel handle.</param>
    /// <param name="buffer">the buffer to write the data.</param>
    /// <param name="length">the number of data written.</param>
    /// <returns>true if a message has been received.</returns>
    public static unsafe bool TryReceiveMessage(NativeRtcChannelHandle handle, Span<byte> buffer, out int length)
    {
        fixed(byte *data = buffer)
        {
            int readLength = buffer.Length;
            int errorCode = NativeRtc.ReceiveMessage(handle.Id, (IntPtr) data, (IntPtr) (&readLength));

            if (errorCode < 0)
            {
                if (errorCode == NativeRtc.ErrorNotAvailable)
                {
                    length = 0;
                    return false;
                }
                
                NativeRtc.ThrowException(errorCode);
            }

            length = readLength;
        }

        return true;
    }
    
    /// <summary>
    ///     Gets the total size of buffered messages waiting to be received.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    /// <returns></returns>
    public static int GetAvailableAmount(NativeRtcChannelHandle handle) => NativeRtc.GetAvailableAmount(handle.Id);

    #region Callbacks

    /// <summary>
    ///     Registers managed callbacks to the channel.
    /// </summary>
    /// <param name="handle">the peer connection handle.</param>
    public static unsafe void AttachCallbacks(NativeRtcChannelHandle handle)
    {
        NativeRtc.SetOpenCallback(handle.Id, &OpenCallback);
        NativeRtc.SetClosedCallback(handle.Id, &ClosedCallback);
        NativeRtc.SetErrorCallback(handle.Id, &ErrorCallback);
        NativeRtc.SetBufferedAmountLowCallback(handle.Id, &BufferedAmountLowCallback);
        NativeRtc.SetAvailableCallback(handle.Id, &AvailableCallback);
    }

    public static unsafe void AttachMessageCallback(NativeRtcChannelHandle handle)
    {
        NativeRtc.SetMessageCallback(handle.Id, &MessageCallback);
    }
    
    public static unsafe void DetachMessageCallback(NativeRtcChannelHandle handle)
    {
        NativeRtc.SetMessageCallback(handle.Id, null);
    }
    
    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void OpenCallback(int id, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnOpen();
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void ClosedCallback(int id, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnClosed();
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void ErrorCallback(int id, IntPtr error, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnError(Marshal.PtrToStringAnsi(error));
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static unsafe void MessageCallback(int id, IntPtr message, int size, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnMessage(size >= 0 
            ? new ReadOnlySpan<byte>((byte*) message, size) 
            : MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*) message));
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void BufferedAmountLowCallback(int id, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnBufferedAmountLow();
    }

    [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvCdecl)})]
    private static void AvailableCallback(int id, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        NativeRtcChannelHandle.FromHandle(handle).Internal_OnAvailable();
    }
    
    #endregion
}