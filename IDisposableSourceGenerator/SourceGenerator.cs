#pragma warning disable CA1031
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDisposableSourceGenerator
{
    [Generator]
    public sealed class IDisposableGenerator : ISourceGenerator
    {
        internal const string AttributeName = nameof(IDisposableGenerator) + "Attribute";

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                //System.Diagnostics.Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var attrCode = new IDisposableGeneratorAttributeTemplate().TransformText();
            context.AddSource(AttributeName + ".cs", attrCode);

            try
            {
                if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

                foreach (var classDeclaration in receiver.Targets)
                {
                    var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(classDeclaration);
                    if (typeSymbol is null) continue;

                    var template = new CodeTemplate(classDeclaration)
                    {
                        Namespace = typeSymbol.ContainingNamespace.ToDisplayString(),
                    };

                    var text = template.TransformText();
                    context.AddSource(typeSymbol.GenerateHintName(), text);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        private sealed class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<ClassDeclarationSyntax> Targets { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;

                var attr = classDeclaration.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.ToString() is nameof(IDisposableGenerator) or AttributeName);
                if (attr is null) return;

                Targets.Add(classDeclaration);
            }
        }
    }
}
