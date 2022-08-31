using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace ResizeWindows.Windows;
internal static class NativeMethods
{
    public static unsafe string GetWindowText(HWND hWnd)
    {
        int cTxtLen = PInvoke.GetWindowTextLength(hWnd);
        if (cTxtLen > 0)
            return GetWindowText(hWnd, cTxtLen);
        return "";
    }
    public static unsafe string GetWindowText(HWND hWnd, int cTxtLen)
    {
        // https://github.com/microsoft/CsWin32/issues/614
        Span<char> text = stackalloc char[cTxtLen + 1];
        fixed (char* pText = text)
        {
            int length = PInvoke.GetWindowText(hWnd, pText, text.Length);
            return new string(pText, 0, length);
        }
    }


    [DllImport("User32", ExactSpelling = true)]
    public static extern unsafe uint GetWindowThreadProcessId(HWND hWnd, out uint lpdwProcessId);

    public static class EventConstants
    {
        public const int EVENT_MIN = 0x00000001;
        public const int EVENT_SYSTEM_SOUND = 0x00000001;
        public const int EVENT_SYSTEM_ALERT = 0x00000002;
        public const int EVENT_SYSTEM_FOREGROUND = 0x00000003;
        public const int EVENT_SYSTEM_MENUSTART = 0x00000004;
        public const int EVENT_SYSTEM_MENUEND = 0x00000005;
        public const int EVENT_SYSTEM_MENUPOPUPSTART = 0x00000006;
        public const int EVENT_SYSTEM_MENUPOPUPEND = 0x00000007;
        public const int EVENT_SYSTEM_CAPTURESTART = 0x00000008;
        public const int EVENT_SYSTEM_CAPTUREEND = 0x00000009;
        public const int EVENT_SYSTEM_MOVESIZESTART = 0x0000000a;
        public const int EVENT_SYSTEM_MOVESIZEEND = 0x0000000b;
        public const int EVENT_SYSTEM_CONTEXTHELPSTART = 0x0000000c;
        public const int EVENT_SYSTEM_CONTEXTHELPEND = 0x0000000d;
        public const int EVENT_SYSTEM_DRAGDROPSTART = 0x0000000e;
        public const int EVENT_SYSTEM_DRAGDROPEND = 0x0000000f;
        public const int EVENT_SYSTEM_DIALOGSTART = 0x00000010;
        public const int EVENT_SYSTEM_DIALOGEND = 0x00000011;
        public const int EVENT_SYSTEM_SCROLLINGSTART = 0x00000012;
        public const int EVENT_SYSTEM_SCROLLINGEND = 0x00000013;
        public const int EVENT_SYSTEM_SWITCHSTART = 0x00000014;
        public const int EVENT_SYSTEM_SWITCHEND = 0x00000015;
        public const int EVENT_SYSTEM_MINIMIZESTART = 0x00000016;
        public const int EVENT_SYSTEM_MINIMIZEEND = 0x00000017;
        public const int EVENT_SYSTEM_DESKTOPSWITCH = 0x00000020;

        public const int EVENT_OBJECT_CREATE = 0x00008000;
        public const int EVENT_OBJECT_DESTROY = 0x00008001;
        public const int EVENT_OBJECT_SHOW = 0x00008002;
        public const int EVENT_OBJECT_HIDE = 0x00008003;
        public const int EVENT_OBJECT_REORDER = 0x00008004;
        public const int EVENT_OBJECT_FOCUS = 0x00008005;
        public const int EVENT_OBJECT_SELECTION = 0x00008006;
        public const int EVENT_OBJECT_SELECTIONADD = 0x00008007;
        public const int EVENT_OBJECT_SELECTIONREMOVE = 0x00008008;
        public const int EVENT_OBJECT_SELECTIONWITHIN = 0x00008009;
        public const int EVENT_OBJECT_STATECHANGE = 0x0000800a;
        public const int EVENT_OBJECT_LOCATIONCHANGE = 0x0000800b;
        public const int EVENT_OBJECT_NAMECHANGE = 0x0000800c;
        public const int EVENT_OBJECT_DESCRIPTIONCHANGE = 0x0000800d;
        public const int EVENT_OBJECT_VALUECHANGE = 0x0000800e;
        public const int EVENT_OBJECT_PARENTCHANGE = 0x0000800f;
        public const int EVENT_OBJECT_HELPCHANGE = 0x00008010;
        public const int EVENT_OBJECT_DEFACTIONCHANGE = 0x00008011;
        public const int EVENT_OBJECT_ACCELERATORCHANGE = 0x00008012;
        public const int EVENT_OBJECT_INVOKED = 0x00008013;
        public const int EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x00008014;
        public const int EVENT_OBJECT_CONTENTSCROLLED = 0x00008015;

        public const int EVENT_CONSOLE_CARET = 0x00004001;
        public const int EVENT_CONSOLE_UPDATE_REGION = 0x00004002;
        public const int EVENT_CONSOLE_UPDATE_SIMPLE = 0x00004003;
        public const int EVENT_CONSOLE_UPDATE_SCROLL = 0x00004004;
        public const int EVENT_CONSOLE_LAYOUT = 0x00004005;
        public const int EVENT_CONSOLE_START_APPLICATION = 0x00004006;
        public const int EVENT_CONSOLE_END_APPLICATION = 0x00004007;

        public const int EVENT_MAX = 0x7fffffff;

        public const uint OBJID_WINDOW = 0x00000000;
        public const uint OBJID_SYSMENU = 0xFFFFFFFF;
        public const uint OBJID_TITLEBAR = 0xFFFFFFFE;
        public const uint OBJID_MENU = 0xFFFFFFFD;
        public const uint OBJID_CLIENT = 0xFFFFFFFC;
        public const uint OBJID_VSCROLL = 0xFFFFFFFB;
        public const uint OBJID_HSCROLL = 0xFFFFFFFA;
        public const uint OBJID_SIZEGRIP = 0xFFFFFFF9;
        public const uint OBJID_CARET = 0xFFFFFFF8;
        public const uint OBJID_CURSOR = 0xFFFFFFF7;
        public const uint OBJID_ALERT = 0xFFFFFFF6;
        public const uint OBJID_SOUND = 0xFFFFFFF5;
        public const int WINEVENT_INCONTEXT = 4;
        public const int WINEVENT_OUTOFCONTEXT = 0;
        public const int WINEVENT_SKIPOWNPROCESS = 2;
        public const int WINEVENT_SKIPOWNTHREAD = 1;
    }
    /// <summary>
    /// Special window handles
    /// </summary>
    public enum SpecialWindowHandles
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Places the window at the top of the Z order.
        /// </summary>
        HWND_TOP = 0,
        /// <summary>
        ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_BOTTOM = 1,
        /// <summary>
        ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_TOPMOST = -1,
        /// <summary>
        ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_NOTOPMOST = -2
        // ReSharper restore InconsistentNaming
    }
}
