using System.Collections.Generic;

namespace ResizeWindows.Model;

public class WindowProcessHandleComparer : IEqualityComparer<WindowProcessHandle>
{
    public bool Equals(WindowProcessHandle? x, WindowProcessHandle? y) => (x, y) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        _ => x.Handle == y.Handle,
    };
    public int GetHashCode(WindowProcessHandle obj) => obj.Handle.GetHashCode();

    private WindowProcessHandleComparer() { }
    private static WindowProcessHandleComparer? _Instance;
    public static WindowProcessHandleComparer Default => _Instance ??= new WindowProcessHandleComparer();
}