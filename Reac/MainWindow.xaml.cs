using ReactiveUI;
using System.Windows;
using System;

namespace Reac
{
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext);

            ViewModel = new MainViewModel();
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Bot();
        }
    }
}
