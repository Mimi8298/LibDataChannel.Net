namespace LibDataChannel.Native.Sdp;

public readonly struct SdpMessage
{
    public SdpType Type { get; }
    public string Content { get; }

    public SdpMessage(SdpType type, string content)
    {
        Type = type;
        Content = content;
    }

    public override string ToString()
    {
        return $"Type: {Type}, Content: {Content}";
    }
}