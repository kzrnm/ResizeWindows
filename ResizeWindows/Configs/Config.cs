using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using static Windows.Win32.PInvoke;

namespace ResizeWindows.Configs;

public record Config(
    ImmutableArray<Preset> Presets = default,
    ImmutableArray<string> IgnoreProcess = default,
    ImmutableArray<OnInitialized> OnInitializeds = default
)
{
    public ImmutableArray<Preset> Presets { get; init; } = Presets.GetOrEmpty();
    public ImmutableArray<string> IgnoreProcess { get; init; } = IgnoreProcess.GetOrEmpty();
    public ImmutableArray<OnInitialized> OnInitializeds { get; init; } = OnInitializeds.GetOrEmpty();
    public Config() : this(Presets: default) { }
}

public record Preset(string Name = "", int Width = 0, int Height = 0)
{
    public Preset() : this(Height: 0) { }
}
public record OnInitialized(string ProcessName = "",
    int Width = 0, int Height = 0,
    int? X = null, int? Y = null)
{
    public OnInitialized() : this(Height: 0) { }
    public bool Move()
    {
        var processesByName = Process.GetProcessesByName(ProcessName);
        if (processesByName.FirstOrDefault()?.MainWindowHandle is IntPtr windowHandle)
        {
            return MoveWindow((global::Windows.Win32.Foundation.HWND)windowHandle, X ?? 0, Y ?? 0, Width, Height, true);
        }
        return false;
    }
}

internal static class ImmutableArrayExtension
{
    public static ImmutableArray<T> GetOrEmpty<T>(this ImmutableArray<T> a)
        => a.IsDefault ? ImmutableArray<T>.Empty : a;
}