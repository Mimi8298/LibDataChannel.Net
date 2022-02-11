namespace LibDataChannel.Native.Connections.Rtc;

public readonly struct RtcIceCandidate
{
    public string Candidate { get; }
    public string Mid { get; }

    public RtcIceCandidate(string candidate, string mid)
    {
        Candidate = candidate;
        Mid = mid;
    }
}