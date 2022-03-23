using System.Runtime.InteropServices;

namespace PictureSnapperService.Utility;

public readonly struct CoTaskMemAllocatedPointer : IDisposable
{
    private readonly IntPtr _pointer;

    public CoTaskMemAllocatedPointer(IntPtr pointer)
    {
        _pointer = pointer;
    }

    public void Dispose()
    {
        Marshal.FreeCoTaskMem(_pointer);
    }

}
