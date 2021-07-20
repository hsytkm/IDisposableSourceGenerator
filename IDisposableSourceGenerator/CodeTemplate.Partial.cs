using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableSourceGenerator
{
    public partial class CodeTemplate
    {
        private const string DefaultTypeName = "SimpleCompositeDisposable";
        private const string DefaultFieldName = "_disposables";

        internal string Namespace { get; set; } = "";
        internal string ClassName { get; }

        internal string CompositeDisposableTypeName { get; }
        internal bool UseDefaultCompositeDisposable { get; }
        internal string CompositeDisposableFieldName { get; }
        internal IDisposableGeneratorOptions Options { get; }

        internal CodeTemplate(ClassDeclarationSyntax classDeclaration, GeneratorArgument genArg)
        {
            ClassName = classDeclaration.GetGenericTypeName();

            var compositeDisposableTypeSymbolName = genArg.CompositeDisposableTypeSymbol?.ToString();
            if (!string.IsNullOrWhiteSpace(compositeDisposableTypeSymbolName))
            {
                CompositeDisposableTypeName = compositeDisposableTypeSymbolName!;
                UseDefaultCompositeDisposable = false;
            }
            else
            {
                CompositeDisposableTypeName = DefaultTypeName;
                UseDefaultCompositeDisposable = true;
            }

            CompositeDisposableFieldName = GetFieldName(genArg.CompositeDisposableFieldName);
            Options = genArg.Options;

            static string GetFieldName(string? s) => !string.IsNullOrWhiteSpace(s) ? s! : DefaultFieldName;
        }

        internal bool HasFlag(IDisposableGeneratorOptions options) => Options.HasFlag(options);

        internal static string ToFullName(IDisposableGeneratorOptions options) => $"{nameof(IDisposableGeneratorOptions)}.{options}";
    }
}
