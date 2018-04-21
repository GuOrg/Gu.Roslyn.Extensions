namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static partial class SymbolExt
    {
        public static bool TrySingleDeclaration(this IFieldSymbol field, CancellationToken cancellationToken, out FieldDeclarationSyntax declaration)
        {
            declaration = null;
            if (field != null &&
                field.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken).FirstAncestorOrSelf<FieldDeclarationSyntax>();
            }

            return declaration != null;
        }

        public static bool TrySingleDeclaration(this IPropertySymbol property, CancellationToken cancellationToken, out PropertyDeclarationSyntax declaration)
        {
            declaration = null;
            if (property != null &&
                property.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken) as PropertyDeclarationSyntax;
            }

            return declaration != null;
        }

        public static bool TrySingleDeclaration(this IMethodSymbol method, CancellationToken cancellationToken, out BaseMethodDeclarationSyntax declaration)
        {
            declaration = null;
            if (method != null &&
                method.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                Debug.Assert(method.AssociatedSymbol == null, "method.AssociatedSymbol == null");
                declaration = reference.GetSyntax(cancellationToken) as BaseMethodDeclarationSyntax;
            }

            return declaration != null;
        }

        public static bool TrySingleDeclaration(this IParameterSymbol parameter, CancellationToken cancellationToken, out ParameterSyntax declaration)
        {
            declaration = null;
            if (parameter != null &&
                parameter.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken) as ParameterSyntax;
            }

            return declaration != null;
        }

        public static bool TrySingleDeclaration(this ILocalSymbol local, CancellationToken cancellationToken, out SyntaxNode declaration)
        {
            declaration = null;
            if (local != null &&
                local.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken);
            }

            return declaration != null;
        }

        public static bool TrySingleDeclaration<T>(this ISymbol symbol, CancellationToken cancellationToken, out T declaration)
            where T : SyntaxNode
        {
            declaration = null;
            if (symbol == null)
            {
                return false;
            }

            if (symbol.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                var syntax = reference.GetSyntax(cancellationToken);
                if (syntax is VariableDeclaratorSyntax declarator)
                {
                    syntax = declarator.FirstAncestorOrSelf<T>();
                }

                declaration = syntax as T;
                return declaration != null;
            }

            return false;
        }
    }
}
