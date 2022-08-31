using ResizeWindows.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace ResizeWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
                DataContext = new MainWindowViewModel();
        }
    }
}
