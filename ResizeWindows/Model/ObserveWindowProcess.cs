using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using static ResizeWindows.Windows.NativeMethods;
using static ResizeWindows.Windows.NativeMethods.EventConstants;
using static Windows.Win32.PInvoke;

namespace ResizeWindows.Model;

public class ObserveWindowProcess : IDisposable
{
    private static ObserveWindowProcess? _Instanse;
    public static ObserveWindowProcess Instanse => _Instanse ??= new();
    private ObserveWindowProcess()
    {
        winEventProc = OberveWindow;
        winEventHook = SetWinEventHook(
            EVENT_OBJECT_SHOW, // eventMin
            EVENT_OBJECT_SHOW, // eventMax
            default(HINSTANCE),             // hmodWinEventProc
            winEventProc,     // lpfnWinEventProc
            0,                       // idProcess
            0,                       // idThread
            WINEVENT_OUTOFCONTEXT);
        InitializeTask = UpdateWindowsAsync();
    }

    private HWINEVENTHOOK winEventHook;
    private WINEVENTPROC winEventProc;
    public Task InitializeTask { get; }

    private WindowProcessHandle[] windows = Array.Empty<WindowProcessHandle>();
    public WindowProcessHandle[] CurrentWindows => windows;

    public event EventHandler<WindowProcessHandle[]>? CurrentWindowsChanged;

    private async void OberveWindow(HWINEVENTHOOK hWinEventHook, uint @event, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (idObject == OBJID_WINDOW && idChild == 0 && GetWindowTextLength(hwnd) > 0)
        {
            _ = GetWindowThreadProcessId(hwnd, out var processId);
            using var process = Process.GetProcessById((int)processId);
            if (process.MainWindowTitle.Length > 0)
            {
                await UpdateWindowsAsync().ConfigureAwait(false);
            }
        }
    }

    private Task UpdateWindowsAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            var list = new WindowProcessHandleProc();
            var paramHandle = GCHandle.Alloc(list);
            EnumWindows(list.EnumWindowsProc, (LPARAM)(nint)paramHandle);
            paramHandle.Free();

            var newWindows = list.ToArray();
            Array.Sort(newWindows.Select(p => p.ProcessName).ToArray(), newWindows);
            if (!Enumerable.SequenceEqual(windows, list, WindowProcessHandleComparer.Default))
            {
                windows = newWindows;
                CurrentWindowsChanged?.Invoke(this, newWindows);
            }
        });
    }
    private class WindowProcessHandleProc : List<WindowProcessHandle>
    {
        public BOOL EnumWindowsProc(HWND hWnd, LPARAM _)
        {
            if (!IsWindowVisible(hWnd)) return true;

            try
            {
                var window = new WindowProcessHandle(hWnd);

                if (window.GetCurrentWindowName() == string.Empty)
                {
                    window.Dispose();
                    return true;
                }

                Add(window);
            }
            catch { }
            return true;
        }
    }

    #region IDisposable Support
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

            }
            UnhookWinEvent(winEventHook);
            winEventHook = default;
            winEventProc = null!;

            disposedValue = true;
        }
    }


    ~ObserveWindowProcess()
    {
        // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        Dispose(false);
    }

    // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
