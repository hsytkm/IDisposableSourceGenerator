#pragma warning disable IDE0057
#pragma warning disable CA1822
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDisposableSourceGenerator
{
    static class StringExtension
    {
        //public static string ToLowerOnlyFirst(this string str) => Char.ToLower(str[0]) + str.Substring(1);
        //public static string ToUpperOnlyFirst(this string str) => Char.ToUpper(str[0]) + str.Substring(1);
        //public static string JoinWithCommas(this IEnumerable<string> ss) => string.Join(", ", ss);
    }

    static class RoslynExtension
    {
        // Code from: https://github.com/YairHalberstadt/stronginject/blob/779a38e7e74b92c87c86ded5d1fef55744d34a83/StrongInject/Generator/RoslynExtensions.cs#L166
        public static string FullName(this INamespaceSymbol @namespace) => @namespace.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

        // Code from: https://github.com/YairHalberstadt/stronginject/blob/779a38e7e74b92c87c86ded5d1fef55744d34a83/StrongInject/Generator/RoslynExtensions.cs#L69
        public static IEnumerable<INamedTypeSymbol> GetContainingTypesAndThis(this INamedTypeSymbol? namedType)
        {
            var current = namedType;
            while (current is not null)
            {
                yield return current;
                current = current.ContainingType;
            }
        }

        // Code from: https://github.com/YairHalberstadt/stronginject/blob/779a38e7e74b92c87c86ded5d1fef55744d34a83/StrongInject/Generator/SourceGenerator.cs#L87
        public static string GenerateHintName(this INamedTypeSymbol container)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(container.ContainingNamespace.FullName());
            foreach (var type in container.GetContainingTypesAndThis().Reverse())
            {
                stringBuilder.Append('.');
                stringBuilder.Append(type.Name);
                if (type.TypeParameters.Length > 0)
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(type.TypeParameters.Length);
                }
            }
            stringBuilder.Append(".g.cs");
            return stringBuilder.ToString();
        }

        //public static string? NullIfEmpty(this string? value) => string.IsNullOrEmpty(value) ? null : value;
    }

    static class SyntaxExtension
    {
        public static string GetGenericTypeName(this TypeDeclarationSyntax typeDecl)
        {
            if (typeDecl.TypeParameterList is null)
                return typeDecl.Identifier.Text;

            var param = string.Join(", ", typeDecl.TypeParameterList.Parameters.Select(p => p.Identifier.Text));
            return typeDecl.Identifier.Text + "<" + param + ">";
        }
    }
}

namespace System.CodeDom.Compiler
{
    public class CompilerError
    {
        public string? ErrorText { get; set; }
        public bool IsWarning { get; set; }
    }

    public class CompilerErrorCollection
    {
        public void Add(CompilerError error) { }
    }
}
