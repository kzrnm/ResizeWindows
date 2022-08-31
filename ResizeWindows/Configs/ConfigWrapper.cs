using ResizeWindows.Mvvm;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ResizeWindows.Configs;

public partial class ConfigWrapper<T> : ObservableObject where T : new()
{
    private ConfigWrapper(T value, string loadPath)
    {
        _value = value;
        LoadPath = loadPath;
    }

    public string LoadPath { get; }

    private T _value;
    public T Value
    {
        set
        {
            if (SetProperty(ref _value, value))
            {
                ConfigUpdated?.Invoke(this, value);
            }
        }
        get => _value;
    }


    public event EventHandler<T>? ConfigUpdated;

    public static async Task<ConfigWrapper<T>> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        T value;
        try
        {
            using var fs = new FileStream(path, FileMode.Open);
            value = await LoadAsync(fs, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            value = new T();
        }

        return new ConfigWrapper<T>(value, path);
    }

    private static async Task<T> LoadAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));

        try
        {
            return (await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            }, cancellationToken: cancellationToken).ConfigureAwait(false)) ?? new();
        }
        catch { }
        return new();
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
        => await SaveAsync(LoadPath, cancellationToken).ConfigureAwait(false);

    public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
    {
        var tmpPath = $"{path}.tmp";
        using (var fs = new FileStream(tmpPath, FileMode.Create))
            await SaveAsync(fs, cancellationToken).ConfigureAwait(false);

        File.Move(tmpPath, path, true);
    }
#if NETFRAMEWORK
    private static class File
    {
        public static void Move(string sourceFileName, string destFileName, bool force)
        {
            if (force)
                new FileInfo(destFileName).Delete();
            System.IO.File.Move(sourceFileName, destFileName);
        }
    }
#endif

    public async Task SaveAsync(Stream stream, CancellationToken cancellationToken)
    {
        await JsonSerializer.SerializeAsync(
            stream,
            Value,
            new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
