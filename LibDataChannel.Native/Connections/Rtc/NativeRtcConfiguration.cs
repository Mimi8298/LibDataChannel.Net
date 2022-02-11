namespace LibDataChannel.Native.Connections.Rtc;

using System.Runtime.InteropServices;

/// <summary>
///     Native configuration to pass to the constructor of <see cref="NativeRtcPeerConnectionHandle"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public ref struct NativeRtcConfiguration
{
    public IntPtr IceServers;
    public int IceServersCount;
    public IntPtr BindAddress;
    public RtcCertificateType CertificateType;
    public RtcTransportPolicy TransportPolicy;
    public bool EnableIceTcp;
    public bool DisableAutoNegotiation;
    public ushort PortRangeBegin;
    public ushort PortRangeEnd;
    public int Mtu;
    public int MaxMessageSize;

    /// <summary>
    ///     Free the native memory allocated for this configuration.
    /// </summary>
    public void Free()
    {
        for (int i = 0; i < IceServersCount; i++)
        {
            Marshal.FreeHGlobal(Marshal.ReadIntPtr(IceServers + i * IntPtr.Size));
        }
        
        Marshal.FreeHGlobal(IceServers);
        Marshal.FreeHGlobal(BindAddress);
    }
}