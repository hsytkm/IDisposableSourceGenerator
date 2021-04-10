using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableSourceGenerator
{
    public partial class CodeTemplate
    {
        private const string DefaultTypeName = "IDisposableSourceGenerator.CompositeDisposable";
        private const string DefaultFieldName = "_disposables";

        internal string Namespace { get; set; } = "";
        internal string ClassName { get; }

        internal string CompositeDisposableTypeName { get; }
        internal string CompositeDisposableFieldName { get; }
        internal IDisposableGeneratorOptions Options { get; }

        internal CodeTemplate(ClassDeclarationSyntax classDeclaration, GeneratorArgument genArg)
        {
            ClassName = classDeclaration.GetGenericTypeName();

            CompositeDisposableTypeName = GetTypeName(genArg.CompositeDisposableTypeSymbol?.ToString());
            CompositeDisposableFieldName = GetFieldName(genArg.CompositeDisposableFieldName);
            Options = genArg.Options;

            static string GetTypeName(string? s) => !string.IsNullOrWhiteSpace(s) ? s! : DefaultTypeName;
            static string GetFieldName(string? s) => !string.IsNullOrWhiteSpace(s) ? s! : DefaultFieldName;
        }

        internal bool HasFlag(IDisposableGeneratorOptions options) => Options.HasFlag(options);

        internal static string ToFullName(IDisposableGeneratorOptions options) => $"{nameof(IDisposableGeneratorOptions)}.{options}";
    }
}
