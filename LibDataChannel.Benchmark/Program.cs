namespace LibDataChannel.Benchmark;

using System.Diagnostics;
using LibDataChannel.Channels.Data;
using LibDataChannel.Connections.Rtc;

public static class Program
{
    private const int PacketSize = 65535;
    
    private static void Main(string[] args)
    {
        MakeConnectionPairBenchmark(1);
        MakeConnectionPairBenchmark(10);
        MakeConnectionPairBenchmark(100);
        MakeBandwidthBenchmark(1_000_000, PacketSize);
        MakeBandwidthBenchmark(10_000_000, PacketSize);
        MakeBandwidthBenchmark(100_000_000, PacketSize);
    }

    private static void MakeConnectionPairBenchmark(int numTest)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numTest; i++)
        {
            MakeConnectionPair(out RtcPeerConnection host, out RtcPeerConnection remote);

            host.Dispose();
            remote.Dispose();
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"{numTest} connection pairs took {stopwatch.ElapsedMilliseconds}ms");
    }

    private static void MakeBandwidthBenchmark(int numBytes, int numBytesPerPacket)
    {
        MakeConnectionPair(out RtcPeerConnection host, out RtcPeerConnection remote);
        
        using (host)
        using (remote)
        {
            RtcDataChannel hostChannel = host.DataChannels[0];
            RtcDataChannel remoteChannel = remote.DataChannels[0];

            AutoResetEvent endEvent = new AutoResetEvent(false);

            int numBytesReceived = 0;

            remoteChannel.OnMessage += message =>
            {
                numBytesReceived += message.Length;

                if (numBytesReceived == numBytes)
                {
                    endEvent.Set();
                }
            };
            
            Task<long> sendingTask = Task.Run(() =>
            {
                ReadOnlySpan<byte> fullPacket = new byte[numBytesPerPacket];
                Stopwatch stopwatch = Stopwatch.StartNew();
                int remainingBytesToSend = numBytes;

                while (remainingBytesToSend >= 1)
                {
                    int numBytesToSend = Math.Min(numBytesPerPacket, remainingBytesToSend);
                    hostChannel.SendMessage(fullPacket[..numBytesToSend]);
                    remainingBytesToSend -= numBytesPerPacket;
                }
                
                endEvent.WaitOne();
                stopwatch.Stop();

                return stopwatch.ElapsedMilliseconds;
            });

            sendingTask.Wait();

            Console.WriteLine($"{numBytesReceived / 1000 / 1000:N}MB received in {sendingTask.Result}ms ({(numBytesReceived / 1000000d) / ((double) sendingTask.Result / 1000) * 8} Mbit/s)");
        }
    }

    private static void MakeConnectionPair(out RtcPeerConnection hostConnection, out RtcPeerConnection remoteConnection)
    {
        AutoResetEvent wait = new AutoResetEvent(false);
        
        RtcPeerConnection host = new RtcPeerConnection();
        RtcPeerConnection remote = new RtcPeerConnection();

        host.OnLocalDescription += sdp =>
        {
            remote.RemoteDescription = sdp;
        };
        remote.OnLocalDescription += sdp =>
        {
            host.RemoteDescription = sdp;
        };
        host.OnLocalCandidate += candidate =>
        {
            remote.AddRemoteCandidate(candidate);
        };
        remote.OnLocalCandidate += candidate =>
        {
            host.AddRemoteCandidate(candidate);
        };
        remote.OnDataChannel += dataChannel =>
        {
            wait.Set();
        };
        
        host.CreateDataChannel("test");

        wait.WaitOne();
        
        hostConnection = host;
        remoteConnection = remote;
    }
}