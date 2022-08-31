using ResizeWindows.Configs;
using ResizeWindows.Model;
using ResizeWindows.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace ResizeWindows.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObserveWindowProcess ObserveWindowProcess { get; }
        public MainWindowViewModel()
        {
            ObserveWindowProcess = ObserveWindowProcess.Instanse;
        }

        public ObservableCollection<Preset> Presets { get; } = new();

        public async void Loaded()
        {
            var config = (await ConfigWrapper<Config>.LoadAsync("Config.json")).Value;
            await ObserveWindowProcess.InitializeTask;
            foreach (var p in config.Presets)
            {
                Presets.Add(p);
            }
            if (!config.Presets.IsEmpty)
                Preset = config.Presets[0];

            _IgnoreProcessName.UnionWith(config.IgnoreProcess);

            bool onInitialized = false;
            foreach (var initialized in config.OnInitializeds)
            {
                onInitialized = initialized.Move();
            }

            if (onInitialized)
            {
                AutoFinish();
            }

            UpdateWindowProcesses();
        }
        public void Closed()
        {
            ObserveWindowProcess.Dispose();
        }

        private int _RemainingForFinish = -1;
        public int RemainingForFinish
        {
            private set => SetProperty(ref _RemainingForFinish, value);
            get => _RemainingForFinish;
        }
        private void AutoFinish()
        {
            const int waitLength = 10;
            RemainingForFinish = waitLength;

            _autoFinishTime = DateTime.Now.AddSeconds(waitLength);
            _autoFinishTimer = new(); // 優先度はDispatcherPriority.Background
            _autoFinishTimer.Interval = TimeSpan.FromSeconds(0.2);
            _autoFinishTimer.Tick += (sender, _) =>
            {
                var timer = sender as DispatcherTimer;
                if (_autoFinishTimer != timer)
                {
                    timer?.Stop();
                    RemainingForFinish = -1;
                    return;
                }

                var remainingSpan = _autoFinishTime - DateTime.Now;
                if (remainingSpan.TotalSeconds < 0)
                {
                    Environment.Exit(0);
                    return;
                }
                RemainingForFinish = (int)remainingSpan.TotalSeconds;
            };
            _autoFinishTimer.Start();
        }
        private DateTime _autoFinishTime;
        private DispatcherTimer? _autoFinishTimer;
        private DelegateCommand? _StopTimerCommand;
        public DelegateCommand StopTimerCommand => _StopTimerCommand ??= new(StopTimer);
        private void StopTimer()
        {
            _autoFinishTimer?.Stop();
            RemainingForFinish = -1;
        }


        public void UpdateWindowProcesses()
        {
            WindowProcesses.Clear();
            foreach (var w in ObserveWindowProcess.CurrentWindows)
            {
                if (w?.ProcessName is { } p && !_IgnoreProcessName.Contains(p))
                    WindowProcesses.Add(w);
            }
        }
        private DelegateCommand? _UpdateWindowProcessesCommand;
        public DelegateCommand UpdateWindowProcessesCommand
            => _UpdateWindowProcessesCommand ??= new(UpdateWindowProcesses);

        public void ApplySize()
        {
            SelectedItem?.ResizeWindow(Width, Height);
        }

        private DelegateCommand? _ApplySizeCommand;
        public DelegateCommand ApplySizeCommand => _ApplySizeCommand ??= new(ApplySize);


        private Preset? _Preset;
        public Preset? Preset
        {
            set
            {
                if (SetProperty(ref _Preset, value) && value is { Height: not 0, Width: not 0 })
                {
                    Height = value.Height;
                    Width = value.Width;
                }
            }
            get => _Preset;
        }

        private readonly HashSet<string> _IgnoreProcessName = new();
        public ObservableCollection<WindowProcessHandle> WindowProcesses { get; } = new();

        private WindowProcessHandle? _SelectedItem;
        public WindowProcessHandle? SelectedItem
        {
            set
            {
                if (SetProperty(ref _SelectedItem, value) && value is not null)
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }
            get => _SelectedItem;
        }

        private int _Width;
        public int Width
        {
            set => SetProperty(ref _Width, value);
            get => _Width;
        }
        private int _Height;
        public int Height
        {
            set => SetProperty(ref _Height, value);
            get => _Height;
        }
    }
}
