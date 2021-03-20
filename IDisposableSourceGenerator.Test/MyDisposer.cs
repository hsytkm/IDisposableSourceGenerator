using IDisposableSourceGenerator;
using System;
using System.Collections.Generic;

namespace IDisposableSourceGenerator.Test
{
    [IDisposableGenerator]
    partial class MyDisposer
    {
        public MyDisposer(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public MyDisposer(IEnumerable<IDisposable> disposables)
        {
            foreach (var d in disposables)
            {
                _disposables.Add(d);
            }
        }
    }

    class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
