#region Using directives

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

#endregion

namespace Reac
{
    public interface IDuck
    {
        IChicken Chicken { get; }
        void Foo();
    }

    public interface IChicken
    {
    }

    public class Chicken : IChicken
    {
    }

    public class Duck : IDuck, INotifyPropertyChanged 
    {
        public IChicken Chicken { get; private set; }

        public Duck()
        {
            Foo();
        }

        public void Foo()
        {
            Chicken = new Chicken();
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