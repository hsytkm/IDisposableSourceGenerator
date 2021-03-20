using System;

namespace IDisposableSourceGenerator.Test
{
    class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; set; }

        public object? NullableValue
        {
            get => _nullableValue;
            set => _nullableValue = value;
        }
        private object? _nullableValue = new();

        public bool IsOpenedUnmanagedResource { get; set; } = true;

        public void Dispose() => IsDisposed = true;
    }
}
