#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

#endregion

namespace Reac
{
    public interface IDuck : INotifyPropertyChanged
    {
        IChicken Chicken { get; }
        void Foo();
    }

    public interface IChicken : INotifyPropertyChanged
    {
        IEnumerable<string> Names { get; }
        void Rename();
    }

    public class Chicken : IChicken
    {
        private readonly List<string> _names = new List<string>();

        #region Implementation of IChicken

        public IEnumerable<string> Names => _names;

        public void Rename()
        {
            _names.Add(Guid.NewGuid().ToString());
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Names)));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    public class Duck : IDuck
    {
        public IChicken Chicken { get; }

        public Duck()
        {
            Chicken = new Chicken();
        }

        public void Foo()
        {
            Chicken.Rename();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Chicken)));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }

    public class MainViewModel : ReactiveObject
    {
        private IDuck _duck;

        public IDuck Duck
        {
            get { return _duck; }
            set { this.RaiseAndSetIfChanged(ref _duck, value); }
        }

        private readonly ObservableAsPropertyHelper<string> dog;
        public string Dog => dog.Value;

        public MainViewModel()
        {
            Duck = new Duck();

            int i = 0;
            dog = this.WhenAnyValue(x => x.Duck.Chicken)
                      .Select(x => $"{i++} => {Guid.NewGuid()}")
                      .ToProperty(this, x => x.Dog, "dog");

            this.WhenAnyValue(x => x.Duck.Chicken)
                .Subscribe(_ => Hit());

            this.WhenAnyValue(x => x.Dog).Subscribe(_ => Hit());
        }

        public Task Bot()
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             while (true)
                                             {
                                                 Thread.Sleep(TimeSpan.FromSeconds(1));
                                                 Duck.Foo();
                                             }
                                         });
        }

        public void Hit()
        {
            Console.WriteLine(Dog);
        }
    }
}