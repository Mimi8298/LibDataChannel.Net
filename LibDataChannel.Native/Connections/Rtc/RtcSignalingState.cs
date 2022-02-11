namespace LibDataChannel.Native.Connections.Rtc;

public enum RtcSignalingState
{
    Stable,
    HaveLocalOffer,
    HaveRemoteOffer,
    HaveLocalPrAnswer,
    HaveRemotePrAnswer,
}