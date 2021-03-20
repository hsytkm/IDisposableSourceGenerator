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

                foreach (var (classDeclaration, options) in receiver.Targets)
                {
                    var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(classDeclaration);
                    if (typeSymbol is null) continue;

                    var template = new CodeTemplate(classDeclaration, options)
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
            internal List<(ClassDeclarationSyntax classDeclaration, IDisposableGeneratorOptions options)> Targets { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;

                var attr = classDeclaration.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.ToString() is nameof(IDisposableGenerator) or AttributeName);
                if (attr is null) return;

                var options = GetOptionsFromAttribute(classDeclaration);

                Targets.Add((classDeclaration, options));
            }

            private static IDisposableGeneratorOptions GetOptionsFromAttribute(ClassDeclarationSyntax classDeclaration)
            {
                var attr = classDeclaration.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.ToString() is nameof(IDisposableGenerator) or AttributeName);

                var argSyntax = attr?.ArgumentList?.Arguments.FirstOrDefault();
                if (argSyntax is null) return IDisposableGeneratorOptions.None;

                // e.g. Options.Flag0 | Options.Flag1 => Flag0 , Flag1
                var parsed = Enum.Parse(typeof(IDisposableGeneratorOptions),
                    argSyntax.Expression.ToString().Replace(nameof(IDisposableGeneratorOptions) + ".", "").Replace("|", ","));

                return (IDisposableGeneratorOptions)parsed;
            }
        }
    }
}
