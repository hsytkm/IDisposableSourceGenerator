using System;
using Xunit;

namespace IDisposableSourceGenerator.Test
{
    [IDisposableGenerator(null, null, IDisposableGeneratorOptions.SetLargeFieldsToNullMethod)]
    partial class FieldsNullDisposer
    {
        private readonly DisposableObject _obj;

        public FieldsNullDisposer(DisposableObject d)
        {
            _obj = d;
            _disposables.Add(d);
        }

        // Set large fields to null when Dispose() or finalizer is invoked.
        protected virtual partial void SetLargeFieldsToNull() => _obj.NullableValue = null;
    }

    public class SetFieldToNullOptionTest
    {
        [Fact]
        public void SetNull_when_Dispose()
        {
            var d = new DisposableObject();
            var disposer = new FieldsNullDisposer(d);

            d.IsDisposed.IsFalse();
            d.NullableValue.IsNotNull();

            disposer.Dispose();         // call SetLargeFieldsToNull()

            d.IsDisposed.IsTrue();
            d.NullableValue.IsNull();   // set field to null
        }

        //[Fact]
        //public void SetNull_when_Finalizer() { }

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
