using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDisposableSourceGenerator
{
    public partial class CodeTemplate
    {
        internal string Namespace { get; set; } = "";
        internal string ClassName { get; }

        internal CodeTemplate(ClassDeclarationSyntax classDeclaration)
        {
            ClassName = classDeclaration.GetGenericTypeName();
        }
    }
}
