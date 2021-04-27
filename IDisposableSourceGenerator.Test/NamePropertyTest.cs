using System;
using Xunit;

namespace IDisposableSourceGenerator.Test
{

    [IDisposableGenerator(default, "_myDisposables", IDisposableGeneratorOptions.None)]
    partial class NamePropertyDisposer
    {
        private readonly DisposableObject _obj;

        public NamePropertyDisposer(DisposableObject d)
        {
            _obj = d;
            _myDisposables.Add(d);  // set field name
        }
    }

    public class NamePropertyTest
    {
        [Fact]
        public void WhenDispose()
        {
            var d = new DisposableObject();
            var disposer = new NamePropertyDisposer(d);

            d.IsDisposed.IsFalse();
            disposer.Dispose();
            d.IsDisposed.IsTrue();
        }

        [Fact]
        public void DonotWork()
        {
            var d = new DisposableObject();
            var disposer = new NamePropertyDisposer(d);

            d.IsDisposed.IsFalse();
            disposer.Dispose();
            d.IsDisposed.IsTrue();
        }
    }
}
