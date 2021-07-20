# IDisposableSourceGenerator

Implement the dispose pattern using SourceGenerator.

[![NuGet](https://img.shields.io/nuget/v/IDisposableSourceGenerator?style=flat-square)](https://www.nuget.org/packages/IDisposableSourceGenerator)

## Introduction

Use this like below.

```csharp
using IDisposableSourceGenerator;

[IDisposableGenerator]
partial class Foo { }    // must be partial class
```

Then the boilerplate code for the disposable pattern will be generated.

``` csharp
partial class Foo : System.IDisposable
{
    protected readonly SimpleCompositeDisposable _disposables =
        new SimpleCompositeDisposable();
    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            // TODO: called on disposing the managed objects.
            //OnDisposing();

            // TODO: dispose managed state (managed objects).
            _disposables.Dispose();
        }
        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //DisposeUnmanagedObjects();

        // TODO: set large fields to null.
        //SetLargeFieldsToNull();

        _disposedValue = true;
    }

    //~Foo() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}
```

Generator declare a `_disposables` field of `SimpleCompositeDisposable` type . You can add disposable objects with it.

The field name `_disposables` can be changed with a generator argument. see **CompositeDisposableFieldName**.

``` csharp
[IDisposableGenerator]
partial class Foo {
    public Foo(IDisposable d) {
        _disposables.Add(d);    // d will be automatically disposed.
    }
}
```

## CompositeDisposableType

You can specify the type of `CompositeDisposable`.

If you don't specified or set default, the default class `IDisposableSourceGenerator.SimpleCompositeDisposable` in the source generator is used.

``` csharp
[IDisposableGenerator(typeof(System.Reactive.Disposables.CompositeDisposable))]
partial class Foo {
    public Foo(IDisposable d) {
        _disposables.Add(d);  // GetType() == typeof(System.Reactive.Disposables.CompositeDisposable)
    }
}
```

## CompositeDisposableFieldName

You can change the name of `CompositeDisposable` field. 

If filed name is default or whitespace, it named `_disposables`.

``` csharp
using IDisposableSourceGenerator;

[IDisposableGenerator(default, "myAwesomeDisposables")]  // CompositeDisposable type is default.
partial class Foo {
    public Foo(IDisposable d) {
        myAwesomeDisposables.Add(d);  // The name specified in the argument.
    }
}
```

## IDisposableGeneratorOptions

Generator generates the `IDisposableGeneratorOptions` that has bit flags of which method to implement.

```csharp
[Flags]
internal enum IDisposableGeneratorOptions {
    None = 0x0000,
    DisposeUnmanagedObjectsMethod = 0x0001,
    SetLargeFieldsToNullMethod = 0x0002,
    OnDisposingMethod = 0x0004,
}
```

Of course,  each option can be set simultaneously.

```csharp
[IDisposableGenerator(default, default, IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod | IDisposableGeneratorOptions.SetLargeFieldsToNullMethod | IDisposableGeneratorOptions.OnDisposingMethod)]
partial class Foo {
    ...
}
```

#### Release a unmanaged object

If you want to release some unmanaged objects, you can use the `IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod` flag.

This option enables the `DisposeUnmanagedObjects` method and Finalizer. It will be called from the `Dispose` method or Finalizer.

``` csharp
[IDisposableGenerator(default, default, IDisposableGeneratorOptions.DisposeUnmanagedObjectsMethod)]
partial class Foo {
    protected virtual partial void DisposeUnmanagedObjects()
    {
        // release some unmanaged objects in this.
    }
}
```

#### Set a large field to null

If you want to set some large fields to null, you can use the `IDisposableGeneratorOptions.SetLargeFieldsToNullMethod` flag.

This option enables the `SetLargeFieldsToNull` method  and Finalizer. It will be called from the `Dispose` method or Finalizer.

``` csharp
[IDisposableGenerator(default, default, IDisposableGeneratorOptions.SetLargeFieldsToNullMethod)]
partial class Foo {
    protected virtual partial void SetLargeFieldsToNull()
    {
        // set some large fields to null in this.
    }
}
```

#### Invoke  when disposing

If you want to do something when disposing the managed objects, you can use the `IDisposableGeneratorOptions.OnDisposing` flag.

This option enables the `OnDisposing` method. It will be called before disposed `CompositeDisposables` from the `Dispose` method.

``` csharp
[IDisposableGenerator(default, default, IDisposableGeneratorOptions.OnDisposingMethod)]
partial class Foo {
    protected virtual partial void OnDisposing()
    {
        // do something when disposing
    }
}
```

## Use in WPF

Maybe WPF app requires editing the `*.csproj` file.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net5.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>

    <!-- for WPF, add the following settings -->
 <IncludePackageReferencesDuringMarkupCompilation>true</IncludePackageReferencesDuringMarkupCompilation>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="IDisposableSourceGenerator" Version="1.0.0" >
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

## License

This library is under the MIT License.

## 謝辞

本プロジェクトでは以下を参考にさせて頂きました。素晴らしいソフトウェアを公開してくださり感謝いたします。

[Cysharp / UnitGenerator](https://github.com/Cysharp/UnitGenerator)

[ufcpp / ValueChangedGenerator](https://github.com/ufcpp/ValueChangedGenerator/)
