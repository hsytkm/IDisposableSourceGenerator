using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableSourceGenerator
{
    public partial class CodeTemplate
    {
        private const string DefaultFieldName = "_disposables";

        internal string Namespace { get; set; } = "";
        internal string ClassName { get; }
        internal IDisposableGeneratorOptions Options { get; }
        internal string CompositeDisposableFieldName { get; }

        internal CodeTemplate(ClassDeclarationSyntax classDeclaration, GeneratorArgument genArg)
        {
            ClassName = classDeclaration.GetGenericTypeName();
            Options = genArg.Options;
            CompositeDisposableFieldName = GetFieldName(genArg.CompositeDisposableFieldName);

            static string GetFieldName(string? s) => !string.IsNullOrWhiteSpace(s) ? s! : DefaultFieldName;
        }

        internal bool HasFlag(IDisposableGeneratorOptions options) => Options.HasFlag(options);

        internal static string ToFullName(IDisposableGeneratorOptions options) => $"{nameof(IDisposableGeneratorOptions)}.{options}";
    }
}
