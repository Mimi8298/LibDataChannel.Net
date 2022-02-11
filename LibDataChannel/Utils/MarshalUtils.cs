namespace LibDataChannel.Utils;

using System.Runtime.InteropServices;

internal static class MarshalUtils
{
    public static IntPtr StringArrayToPtr(string[] array)
    {
        if (array == null)
            return IntPtr.Zero;
        
        IntPtr ptr = Marshal.AllocHGlobal(array.Length * IntPtr.Size);
        
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.WriteIntPtr(ptr, i * IntPtr.Size, Marshal.StringToHGlobalAnsi(array[i]));
        }

        return ptr;
    }
}