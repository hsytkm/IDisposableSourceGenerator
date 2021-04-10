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

                foreach (var (classDeclaration, attributeSyntax) in receiver.Targets)
                {
                    var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(classDeclaration);
                    if (typeSymbol is null) continue;

                    var genArgs = new GeneratorArgument(model, attributeSyntax);
                    var template = new CodeTemplate(classDeclaration, genArgs)
                    {
                        Namespace = typeSymbol.ContainingNamespace.ToDisplayString(),
                    };

                    if (context.CancellationToken.IsCancellationRequested) return;

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
            internal List<(ClassDeclarationSyntax classDeclaration, AttributeSyntax attributeSyntax)> Targets { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;

                var attr = classDeclaration.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.ToString() is nameof(IDisposableGenerator) or AttributeName);
                if (attr is null) return;

                Targets.Add((classDeclaration, attr));
            }
        }
    }

    internal record GeneratorArgument
    {
        public ITypeSymbol? CompositeDisposableTypeSymbol { get; }
        public string? CompositeDisposableFieldName { get; }
        public IDisposableGeneratorOptions Options { get; }

        public GeneratorArgument(SemanticModel model, AttributeSyntax attributeSyntax)
        {
            var args = attributeSyntax?.ArgumentList?.Arguments;
            if (args is not SeparatedSyntaxList<AttributeArgumentSyntax> attrArgs) return;

            for (var i = 0; i < attrArgs.Count; i++)
            {
                var arg = attrArgs[i];
                var expr = arg.Expression;

                if (i == 0)    // Type
                {
                    ITypeSymbol? typeSymbol = null;
                    if (expr is TypeOfExpressionSyntax typeOfExpr)
                        typeSymbol = model.GetSymbolInfo(typeOfExpr.Type).Symbol as ITypeSymbol;

                    CompositeDisposableTypeSymbol = typeSymbol;
                }
                else if (i == 1)    // string
                {
                    CompositeDisposableFieldName = model.GetConstantValue(expr).Value?.ToString();
                }
                else if (i == 2)    // IDisposableGeneratorOptions
                {
                    Options = GetOptions(arg);
                }
            }
        }

        private static IDisposableGeneratorOptions GetOptions(AttributeArgumentSyntax? attributeArgumentSyntax)
        {
            if (attributeArgumentSyntax is null) return IDisposableGeneratorOptions.None;

            // e.g. Options.Flag0 | Options.Flag1 => Flag0 , Flag1
            var parsed = Enum.Parse(typeof(IDisposableGeneratorOptions),
                attributeArgumentSyntax.Expression.ToString().Replace(nameof(IDisposableGeneratorOptions) + ".", "").Replace("|", ","));

            return (IDisposableGeneratorOptions)parsed;
        }
    }
}
