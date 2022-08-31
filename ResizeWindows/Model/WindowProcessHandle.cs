using System;
using System.Diagnostics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static ResizeWindows.Windows.NativeMethods;
using static Windows.Win32.PInvoke;

namespace ResizeWindows.Model;

public class WindowProcessHandle : IDisposable
{
    internal HWND Handle { get; private set; }
    public string ProcessName { get; }
    public string WindowName { get; }

    public int PositionX { get; }
    public int PositionY { get; }
    public int Height { get; }
    public int Width { get; }

    private static string GetProcessName(HWND handle)
    {
        _ = GetWindowThreadProcessId(handle, out var processId);
        using var process = Process.GetProcessById((int)processId);
        return process.ProcessName;
    }
    internal WindowProcessHandle(HWND handle)
    {
        Handle = handle;
        ProcessName = GetProcessName(handle);
        WindowName = GetWindowText(handle);

        _ = GetWindowRect(handle, out var rect);
        PositionX = rect.left;
        PositionY = rect.top;
        Width = rect.right - rect.left;
        Height = rect.bottom - rect.top;
    }

    public bool IsActive => IsWindow(Handle);
    public override string ToString() => $"{ProcessName}|{WindowName}";
    public override int GetHashCode() => Handle.GetHashCode();
    public string GetCurrentWindowName() => GetWindowText(Handle);
    public void ResizeWindow(int width, int height)
    {
        SetWindowPos(Handle,
            (HWND)(nint)SpecialWindowHandles.HWND_TOP,
            0, 0, width, height,
            SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
    }
    private bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            Handle = default;
            disposed = true;
        }
    }
}
