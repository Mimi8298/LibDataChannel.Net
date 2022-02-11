namespace LibDataChannel.Connections.Rtc;

using System.Runtime.InteropServices;
using LibDataChannel.Native.Connections.Rtc;
using LibDataChannel.Utils;

/// <summary>
///     Represents the initial configuration of a RTC peer connection.
/// </summary>
public class RtcPeerConfiguration
{
    /// <summary>
    ///     Ice servers to use. Each ice server must respect the following format: ("stun"|"turn"|"turns"):(login ":" password "@")(hostname ":" port)?(transport "=" ("udp"|"tcp"|"tls"))
    ///     Example: stun:stun.l.google.com:19302
    ///     Example: turn:myuser:12345678@turnserver.org:3478?transport=udp
    /// </summary>
    public string[] IceServers { get; set; }
    
    /// <summary>
    ///     Bind address to use.
    /// </summary>
    public string BindAddress { get; set; }
    
    /// <summary>
    ///     Type of certificate to use.
    /// </summary>
    public RtcCertificateType CertificateType { get; set; }
    
    /// <summary>
    ///     Transport policy to use.
    /// </summary>
    public RtcTransportPolicy TransportPolicy { get; set; }
    
    /// <summary>
    ///     Disable the auto negotiation when a data channel is created.
    /// </summary>
    public bool DisableAutoNegotiation { get; set; }
    
    /// <summary>
    ///     Start of the port range to use.
    /// </summary>
    public ushort PortRangeBegin { get; set; }
    
    /// <summary>
    ///     End of the port range to use (include).
    /// </summary>
    public ushort PortRangeEnd { get; set; }
    
    /// <summary>
    ///     Maximum transmission unit to use for the packets.
    /// </summary>
    public int Mtu { get; set; }
    
    /// <summary>
    ///     Maximum message size to use for the packets.
    /// </summary>
    public int MaxMessageSize { get; set; }

    /// <summary>
    ///     Allocates a <see cref="NativeRtcConfiguration"/> from this instance.
    /// </summary>
    /// <returns>the native configuration.</returns>
    internal NativeRtcConfiguration AllocNative()
    {
        return new NativeRtcConfiguration
        {
            IceServers = MarshalUtils.StringArrayToPtr(IceServers),
            IceServersCount = IceServers?.Length ?? 0,
            BindAddress = Marshal.StringToHGlobalAnsi(BindAddress),
            CertificateType =  CertificateType,
            TransportPolicy = TransportPolicy,
            EnableIceTcp = false,
            DisableAutoNegotiation = DisableAutoNegotiation,
            PortRangeBegin = PortRangeBegin,
            PortRangeEnd = PortRangeEnd,
            Mtu = Mtu,
            MaxMessageSize = MaxMessageSize
        };
    }
}