namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The safe versions handle situations like partial classes when the node is not in the same syntax tree.
    /// </summary>
    public static class SemanticModelExt
    {
        public static bool IsEither<T1, T2>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
            where T1 : ISymbol
            where T2 : ISymbol
        {
            return semanticModel.GetSymbolSafe(node, cancellationToken).IsEither<T1, T2>() ||
                   semanticModel.GetDeclaredSymbolSafe(node, cancellationToken).IsEither<T1, T2>() ||
                   semanticModel.GetTypeInfoSafe(node, cancellationToken).Type.IsEither<T1, T2>();
        }

        public static ISymbol GetSymbolSafe(this SemanticModel semanticModel, AwaitExpressionSyntax node, CancellationToken cancellationToken)
        {
            return semanticModel.GetSymbolSafe(node.Expression, cancellationToken);
        }

        public static IMethodSymbol GetSymbolSafe(this SemanticModel semanticModel, ConstructorInitializerSyntax node, CancellationToken cancellationToken)
        {
            return (IMethodSymbol)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        public static ISymbol GetSymbolSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is AwaitExpressionSyntax awaitExpression)
            {
                return GetSymbolSafe(semanticModel, awaitExpression, cancellationToken);
            }

            return semanticModel.SemanticModelFor(node)
                                ?.GetSymbolInfo(node, cancellationToken)
                                .Symbol;
        }

        public static IFieldSymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, FieldDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (node?.Declaration == null)
            {
                return null;
            }

            if (node.Declaration.Variables.TrySingle(out var variable))
            {
                return (IFieldSymbol)semanticModel.SemanticModelFor(node)
                                                  ?.GetDeclaredSymbol(variable, cancellationToken);
            }

            return null;
        }

        public static IMethodSymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, ConstructorDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return (IMethodSymbol)semanticModel.SemanticModelFor(node)
                                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static ISymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, BasePropertyDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return semanticModel.SemanticModelFor(node)
                                ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static IPropertySymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, PropertyDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return (IPropertySymbol)semanticModel.SemanticModelFor(node)
                                                 ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static IPropertySymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, IndexerDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return (IPropertySymbol)semanticModel.SemanticModelFor(node)
                                                 ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static IMethodSymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, MethodDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return (IMethodSymbol)semanticModel.SemanticModelFor(node)
                                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static ITypeSymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, TypeDeclarationSyntax node, CancellationToken cancellationToken)
        {
            return (ITypeSymbol)semanticModel.SemanticModelFor(node)
                                             ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static IParameterSymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, ParameterSyntax node, CancellationToken cancellationToken)
        {
            return (IParameterSymbol)GetDeclaredSymbolSafe(semanticModel, (SyntaxNode)node, cancellationToken);
        }

        public static ISymbol GetDeclaredSymbolSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is FieldDeclarationSyntax fieldDeclaration)
            {
                return GetDeclaredSymbolSafe(semanticModel, fieldDeclaration, cancellationToken);
            }

            return semanticModel.SemanticModelFor(node)
                                ?.GetDeclaredSymbol(node, cancellationToken);
        }

        public static Optional<object> GetConstantValueSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            return semanticModel.SemanticModelFor(node)
                                ?.GetConstantValue(node, cancellationToken) ?? default(Optional<object>);
        }

        public static bool TryGetConstantValue<T>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken, out T value)
        {
            var optional = GetConstantValueSafe(semanticModel, node, cancellationToken);
            if (optional.HasValue)
            {
                value = (T)optional.Value;
                return true;
            }

            value = default(T);
            return false;
        }

        public static TypeInfo GetTypeInfoSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            return semanticModel.SemanticModelFor(node)
                                ?.GetTypeInfo(node, cancellationToken) ?? default(TypeInfo);
        }

        /// <summary>
        /// Gets the semantic model for <paramref name="expression"/>
        /// This can be needed for partial classes.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>The semantic model that corresponds to <paramref name="expression"/></returns>
        public static SemanticModel SemanticModelFor(this SemanticModel semanticModel, SyntaxNode expression)
        {
            if (semanticModel == null ||
                expression == null ||
                expression.IsMissing)
            {
                return null;
            }

            if (ReferenceEquals(semanticModel.SyntaxTree, expression.SyntaxTree))
            {
                return semanticModel;
            }

            if (semanticModel.Compilation.ContainsSyntaxTree(expression.SyntaxTree))
            {
                return semanticModel.Compilation.GetSemanticModel(expression.SyntaxTree);
            }

            foreach (var metadataReference in semanticModel.Compilation.References)
            {
                if (metadataReference is CompilationReference compilationReference)
                {
                    if (compilationReference.Compilation.ContainsSyntaxTree(expression.SyntaxTree))
                    {
                        return compilationReference.Compilation.GetSemanticModel(expression.SyntaxTree);
                    }
                }
            }

            return null;
        }
    }
}
