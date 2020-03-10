using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp39
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly BooleanNotifier _dbValueUpdating = new BooleanNotifier();
        public ReactiveCommand<string> InitCommand { get; }
        public ReadOnlyReactiveProperty<string> CommandText { get; }
        private ReactiveProperty<string> DatabaseValue { get; }

        public ReactiveProperty<string> InputValue { get; }

        public MainWindowViewModel()
        {
            DatabaseValue = new ReactiveProperty<string>();
            InitCommand = new ReactiveCommand<string>()
                .WithSubscribe(isDatabaseEmpty =>
                {
                    _dbValueUpdating.SwitchValue();
                    using (Disposable.Create(() => _dbValueUpdating.SwitchValue()))
                    {
                        DatabaseValue.Value = bool.Parse(isDatabaseEmpty) ? null : "xxxx";
                        DatabaseValue.ForceNotify();
                    }
                });

            InputValue = DatabaseValue.ToReactiveProperty();

            CommandText = Observable.Merge(
                DatabaseValue.Select(x => IsDbValueEmpty(x)).Select(x => x ? "A" : "B"),
                InputValue.Where(_ => !_dbValueUpdating.Value).Select(_ => "A"))
                .ToReadOnlyReactiveProperty();
        }

        private static bool IsDbValueEmpty(string x) => string.IsNullOrEmpty(x) || x == "0";
    }
}
