
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public IChicken Chicken { get; }

        public Duck()
        {
            Foo();
        }

        public void Foo()
        {
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

            dog = this.WhenAnyValue(x => x.Duck.Chicken)
			          .Select(x => Guid.NewGuid().ToString())
                    .ToProperty(this, x => x.Dog, "dog");

            this.WhenAnyValue(x => x.Duck.Chicken)
                .Subscribe(_ => Hit());

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Duck.Foo();
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Duck.Foo();
            });
        }

        public void Hit()
        {
            Console.WriteLine(Dog);
        }
    }
}
