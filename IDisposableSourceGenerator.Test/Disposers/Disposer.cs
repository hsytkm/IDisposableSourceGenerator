using IDisposableSourceGenerator;
using System;
using System.Collections.Generic;

namespace IDisposableSourceGenerator.Test
{
    [IDisposableGenerator]
    partial class SimpleDisposer
    {
        public SimpleDisposer(IDisposable disposable) => _disposables.Add(disposable);

        public SimpleDisposer(IEnumerable<IDisposable> disposables)
        {
            foreach (var d in disposables)
            {
                _disposables.Add(d);
            }
        }
    }

    [IDisposableGenerator(IDisposableGeneratorOptions.None)]
    partial class DisposerNone
    {
        public DisposerNone(IDisposable disposable) => _disposables.Add(disposable);
    }

    [IDisposableGenerator(IDisposableGeneratorOptions.DisposeManagedObjectsMethod)]
    partial class DisposerManaged
    {
        public DisposerManaged(IDisposable disposable) => _disposables.Add(disposable);

        protected virtual partial void DisposeManagedObjects()
        {
        }
    }

    [IDisposableGenerator(IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod)]
    partial class DisposerUnmanaged
    {
        public DisposerUnmanaged(IDisposable disposable) => _disposables.Add(disposable);

        protected virtual partial void DisposeUnmanagedObjects()
        {
        }
    }

    [IDisposableGenerator(IDisposableGeneratorOptions.SetLargeFieldsToNullMethod)]
    partial class DisposerFieldsNull
    {
        public DisposerFieldsNull(IDisposable disposable) => _disposables.Add(disposable);

        protected virtual partial void SetLargeFieldsToNull()
        {
        }
    }

    [IDisposableGenerator(IDisposableGeneratorOptions.DisposeManagedObjectsMethod | IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod | IDisposableGeneratorOptions.SetLargeFieldsToNullMethod)]
    partial class DisposerAllOptions
    {
        public DisposerAllOptions(IDisposable disposable) => _disposables.Add(disposable);

        protected virtual partial void DisposeManagedObjects()
        {
        }

        protected virtual partial void DisposeUnmanagedObjects()
        {
        }

        protected virtual partial void SetLargeFieldsToNull()
        {
        }
    }

}
