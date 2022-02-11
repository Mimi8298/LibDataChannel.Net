namespace LibDataChannel.Native;

using System.Runtime.InteropServices;
using LibDataChannel.Native.Utils;

public abstract class NativeRtcHandle : IDisposable
{
    private GCHandle Handle { get; }
    protected IntPtr HandlePtr { get; }
    
    public bool Disposed { get; private set; }
    public bool FromCallback => ThreadUtils.IsRtcThread();
    
    public object SyncRoot => this;

    public NativeRtcHandle()
    {
        Handle = GCHandle.Alloc(this, GCHandleType.WeakTrackResurrection);
        HandlePtr = (IntPtr) Handle;
    }
    
    ~NativeRtcHandle()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            Disposed = true;
            
            try
            {
                OnDispose();
            }
            finally
            {
                if (FromCallback)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Free();
                        Handle.Free();
                    });
                }
                else
                {
                    Free();
                    Handle.Free();         
                }
            }
        }
    }

    protected abstract void OnDispose();
    protected abstract void Free();
}