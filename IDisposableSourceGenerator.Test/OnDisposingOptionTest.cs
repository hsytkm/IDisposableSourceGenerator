using System;
using Xunit;

namespace IDisposableSourceGenerator.Test
{
    [IDisposableGenerator(default, default, IDisposableGeneratorOptions.OnDisposingMethod)]
    partial class OnDisposingDisposer
    {
        private readonly DisposableObject _obj;

        public OnDisposingDisposer(DisposableObject d)
        {
            _obj = d;
            _disposables.Add(d);
        }

        protected virtual partial void OnDisposing() => _obj.NullableValue = null;
    }

    public class OnDisposingOptionTest
    {
        [Fact]
        public void SetNull_when_Dispose()
        {
            var d = new DisposableObject();
            var disposer = new OnDisposingDisposer(d);

            d.IsDisposed.IsFalse();
            d.NullableValue.IsNotNull();

            disposer.Dispose();         // call OnDisposing()

            d.IsDisposed.IsTrue();
            d.NullableValue.IsNull();   // set field to null
        }

        [Fact]
        public void SetNull_do_not_work()
        {
            var d = new DisposableObject();
            var disposer = new NoneOptionDisposer(d);

            d.IsDisposed.IsFalse();
            d.NullableValue.IsNotNull();

            disposer.Dispose();

            d.IsDisposed.IsTrue();
            d.NullableValue.IsNotNull();    // Did not set null
        }

    }
}
