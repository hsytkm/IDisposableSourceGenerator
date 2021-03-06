using IDisposableSourceGenerator;
using System;
using System.Collections.Generic;

namespace IDisposableSourceGenerator.Test
{
    [IDisposableGenerator]
    partial class DefaultDisposer
    {
        public DefaultDisposer(IDisposable disposable) => _disposables.Add(disposable);

        public DefaultDisposer(IEnumerable<IDisposable> disposables)
        {
            foreach (var d in disposables)
            {
                _disposables.Add(d);
            }
        }
    }

    [IDisposableGenerator(default, default, IDisposableGeneratorOptions.None)]
    partial class NoneOptionDisposer
    {
        public NoneOptionDisposer(IDisposable disposable) => _disposables.Add(disposable);
    }

    [IDisposableGenerator(default, default, IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod | IDisposableGeneratorOptions.SetLargeFieldsToNullMethod)]
    partial class AllOptionsDisposer
    {
        public AllOptionsDisposer(IDisposable disposable) => _disposables.Add(disposable);

        protected virtual partial void DisposeUnmanagedObjects() { }
        protected virtual partial void SetLargeFieldsToNull() { }
    }

}
