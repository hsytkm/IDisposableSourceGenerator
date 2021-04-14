using System;

namespace IDisposableSourceGenerator
{
    // same as Generated Options(check IDisposableGeneratorAttributeTemplate.tt).
    [Flags]
    internal enum IDisposableGeneratorOptions
    {
        None = 0x0000,
        DisposeUnmanagedObjectsMethod = 0x0001,
        SetLargeFieldsToNullMethod = 0x0002,
        OnDisposingMethod = 0x0004,
    }
}
