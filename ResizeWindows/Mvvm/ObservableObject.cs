using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResizeWindows.Mvvm;

public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new(propertyName));
            return true;
        }
        return false;
    }
}
