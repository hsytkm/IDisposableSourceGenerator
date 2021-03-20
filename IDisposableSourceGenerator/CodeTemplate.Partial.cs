using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableSourceGenerator
{
    public partial class CodeTemplate
    {
        internal string Namespace { get; set; } = "";
        internal string ClassName { get; }
        internal IDisposableGeneratorOptions Options { get; }

        internal CodeTemplate(ClassDeclarationSyntax classDeclaration, IDisposableGeneratorOptions options)
        {
            ClassName = classDeclaration.GetGenericTypeName();
            Options = options;
        }

        internal bool HasFlag(IDisposableGeneratorOptions options) => Options.HasFlag(options);

        internal static string ToFullName(IDisposableGeneratorOptions options) => $"{nameof(IDisposableGeneratorOptions)}.{options}";
    }
}
