namespace LibDataChannel.Native.Exceptions;

using System;

internal class RtcException : Exception
{
    internal RtcException(string message) : base(message)
    {
    }
}