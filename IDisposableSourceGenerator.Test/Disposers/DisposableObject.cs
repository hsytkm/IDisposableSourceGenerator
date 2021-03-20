using System;

namespace IDisposableSourceGenerator.Test
{
    class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; set; }

        public void Dispose() => IsDisposed = true;
    }
}
