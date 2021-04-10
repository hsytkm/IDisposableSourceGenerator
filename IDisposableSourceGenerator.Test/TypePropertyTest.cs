using System;
using Xunit;

namespace IDisposableSourceGenerator.Test
{

    [IDisposableGenerator(typeof(System.Reactive.Disposables.CompositeDisposable), "_reactiveDisposable", IDisposableGeneratorOptions.None)]
    partial class TypePropertyDisposer
    {
        private readonly DisposableObject _obj;

        public TypePropertyDisposer(DisposableObject d)
        {
            _obj = d;
            
            if (_reactiveDisposable.GetType() == typeof(System.Reactive.Disposables.CompositeDisposable))
            {
                _reactiveDisposable.Add(d);  // set field name
            }
        }
    }

    public class TypePropertyTest
    {
        [Fact]
        public void WhenDispose()
        {
            var d = new DisposableObject();
            var disposer = new TypePropertyDisposer(d);

            d.IsDisposed.IsFalse();
            disposer.Dispose();
            d.IsDisposed.IsTrue();
        }

        [Fact]
        public void DonotWork()
        {
            var d = new DisposableObject();
            var disposer = new TypePropertyDisposer(d);

            d.IsDisposed.IsFalse();
            disposer.Dispose();
            d.IsDisposed.IsTrue();
        }
    }
}
