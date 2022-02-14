namespace LibDataChannel.Test;

using System.Linq;
using System.Text;
using System.Threading;
using LibDataChannel.Channels.Data;
using LibDataChannel.Connections.Rtc;
using LibDataChannel.Native.Connections.Rtc;
using NUnit.Framework;

public class RtcPeerConnection_Connectivity
{
    private RtcPeerConnection _host;
    private RtcPeerConnection _remote;
    
    [SetUp]
    public void Setup()
    {
        RtcPeerConfiguration configuration = new RtcPeerConfiguration
        {
            DisableAutoNegotiation = true
        };
        
        _host = new RtcPeerConnection(configuration);
        _remote = new RtcPeerConnection(configuration);

        _host.OnLocalDescription += sdp =>
        {
            _remote.RemoteDescription = sdp;
            _remote.CreateAnswer();
        };
        _remote.OnLocalDescription += sdp =>
        {
            _host.RemoteDescription = sdp;
        };
        _host.OnLocalCandidate += candidate =>
        {
            _remote.AddRemoteCandidate(candidate);
        };
        _remote.OnLocalCandidate += candidate =>
        {
            _host.AddRemoteCandidate(candidate);
        };
        
        AutoResetEvent wait1 = new AutoResetEvent(false);
        AutoResetEvent wait2 = new AutoResetEvent(false);
        AutoResetEvent wait3 = new AutoResetEvent(false);
        
        _host.OnStateChange += state =>
        {
            if (state == RtcState.Connected)
            {
                wait1.Set();
            }
        };
        _remote.OnStateChange += state =>
        {
            if (state == RtcState.Connected)
            {
                wait2.Set();
            }
        };
        _remote.OnDataChannel += channel =>
        {
            wait3.Set();
        };

        _host.CreateDataChannel("test", new RtcDataChannelInit
        {
            Protocol = "test"
        });
        _host.CreateOffer();
        
        wait1.WaitOne(1000);
        wait2.WaitOne(1000);
        wait3.WaitOne(1000);
    }
    
    [TearDown]
    public void TearDown()
    {
        _host.Dispose();
        _remote.Dispose();
    }

    [Test]
    public void Connection()
    {
        Assert.AreEqual(RtcState.Connected, _host.State);
        Assert.AreEqual(RtcState.Connected, _remote.State);
    }

    [Test]
    public void Closing()
    {
        _host.Dispose();
        _remote.Dispose();
        
        Assert.AreEqual(RtcState.Closed, _host.State);
        Assert.AreEqual(RtcState.Closed, _remote.State);
        Assert.AreEqual(0, _host.DataChannels.Count);
        Assert.AreEqual(0, _remote.DataChannels.Count);
    }

    [Test]
    public void DataChannel_OptionEquality()
    {
        Assert.AreEqual(_host.DataChannels.Count, _remote.DataChannels.Count);

        foreach ((RtcDataChannel c1, RtcDataChannel c2) in _host.DataChannels.Zip(_remote.DataChannels))
        {
            Assert.AreEqual(c1.Label, c2.Label);
            Assert.AreEqual(c1.Protocol, c2.Protocol);
            Assert.AreEqual(c1.Reliability.Ordered, c2.Reliability.Ordered);
            Assert.AreEqual(c1.Reliability.Reliable, c2.Reliability.Reliable);
            Assert.AreEqual(c1.Reliability.MaxRetransmits, c2.Reliability.MaxRetransmits);
            Assert.AreEqual(c1.Reliability.MaxPacketLifeTime, c2.Reliability.MaxPacketLifeTime);
        }
    }
    
    [Test]
    public void DataChannel_LazyCreation()
    {
        AutoResetEvent wait = new AutoResetEvent(false);
        
        _host.CreateDataChannel("test2");
        _remote.OnDataChannel += channel =>
        {
            if (channel.Label == "test2")
            {
                wait.Set();
            }
        };
        
        wait.WaitOne(1000);
        
        Assert.AreEqual(_host.DataChannels.Count, _remote.DataChannels.Count);
    }

    [Test]
    public void DataChannel_DataExchange()
    {
        RtcDataChannel hostChannel = _host.DataChannels.First();
        RtcDataChannel remoteChannel = _remote.DataChannels.First();

        const string messageFromHost = "Hello from host";
        const string messageFromRemote = "Hello from remote";
        
        AutoResetEvent wait1 = new AutoResetEvent(false);
        AutoResetEvent wait2 = new AutoResetEvent(false);
        
        hostChannel.OnMessage += message =>
        {
            string messageString = Encoding.UTF8.GetString(message);
            
            if (messageString == messageFromRemote)
            {
                wait1.Set();
            }
        };
        remoteChannel.OnMessage += message =>
        {
            string messageString = Encoding.UTF8.GetString(message);
            
            if (messageString == messageFromHost)
            {
                wait2.Set();
            }
        };
        
        hostChannel.SendMessage(Encoding.UTF8.GetBytes(messageFromHost));
        remoteChannel.SendMessage(Encoding.UTF8.GetBytes(messageFromRemote));
        
        Assert.True(wait1.WaitOne(1000));
        Assert.True(wait2.WaitOne(1000));
    }
}