using System;
using Xunit;

namespace IDisposableSourceGenerator.Test
{

    [IDisposableGenerator(null, null, IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod)]
    partial class UnmanagedOptionDisposer
    {
        private readonly DisposableObject _obj;

        public UnmanagedOptionDisposer(DisposableObject d)
        {
            _obj = d;
            _disposables.Add(d);
        }

        // Dispose unmanaged objects when Dispose() or finalizer is invoked.
        protected virtual partial void DisposeUnmanagedObjects() => _obj.IsOpenedUnmanagedResource = false;
    }

    public class UnmanagedOptionTest
    {
        [Fact]
        public void Unmanaged_when_Dispose()
        {
            var d = new DisposableObject();
            var disposer = new UnmanagedOptionDisposer(d);

            d.IsDisposed.IsFalse();
            d.IsOpenedUnmanagedResource.IsTrue();

            disposer.Dispose();     // call DisposeUnmanagedObjects()

            d.IsDisposed.IsTrue();
            d.IsOpenedUnmanagedResource.IsFalse();   // release unmanaged resources
        }

        //[Fact]
        //public void Unmanaged_when_Finalizer() { }

        [Fact]
        public void Unmanaged_do_not_work()
        {
            var d = new DisposableObject();
            var disposer = new NoneOptionDisposer(d);

            d.IsDisposed.IsFalse();
            d.IsOpenedUnmanagedResource.IsTrue();

            disposer.Dispose();

            d.IsDisposed.IsTrue();
            d.IsOpenedUnmanagedResource.IsTrue();   // Did not release resources
        }

    }
}
