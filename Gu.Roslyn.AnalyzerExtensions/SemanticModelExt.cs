// ReSharper disable UnusedMember.Global
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The safe versions handle situations like partial classes when the node is not in the same syntax tree.
    /// </summary>
    public static partial class SemanticModelExt
    {
        /// <summary>
        /// Try getting the <see cref="ISymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <typeparam name="TSymbol">The symbol</typeparam>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="symbol">The symbol if found</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol<TSymbol>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken, out TSymbol symbol)
            where TSymbol : class, ISymbol
        {
            symbol = GetSymbolSafe(semanticModel, node, cancellationToken) as TSymbol ??
                     GetDeclaredSymbolSafe(semanticModel, node, cancellationToken) as TSymbol;
            return symbol != null;
        }

        /// <summary>
        /// Try getting the GetConstantValue for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <typeparam name="T">The symbol</typeparam>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="value">The symbol if found</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetConstantValue<T>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken, out T value)
            where T : class, ISymbol
        {
            if (semanticModel.GetConstantValueSafe(node, cancellationToken) is Optional<object> optional &&
                optional.HasValue &&
                optional.Value is T temp)
            {
                value = temp;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try getting the GetTypeInfo for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="type">The symbol if found</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetTypeInfo(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken, out ITypeSymbol type)
        {
            if (semanticModel.GetTypeInfoSafe(node, cancellationToken).Type is ITypeSymbol temp)
            {
                type = temp;
                return true;
            }

            type = null;
            return false;
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISymbol"/> or null</returns>
        public static ISymbol GetSymbolSafe(this SemanticModel semanticModel, AwaitExpressionSyntax node, CancellationToken cancellationToken)
        {
            return semanticModel.GetSymbolSafe(node.Expression, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISymbol"/> or null</returns>
        public static IMethodSymbol GetSymbolSafe(this SemanticModel semanticModel, ConstructorInitializerSyntax node, CancellationToken cancellationToken)
        {
            return (IMethodSymbol)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISymbol"/> or null</returns>
        public static IMethodSymbol GetSymbolSafe(this SemanticModel semanticModel, ObjectCreationExpressionSyntax node, CancellationToken cancellationToken)
        {
            return (IMethodSymbol)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISymbol"/> or null</returns>
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

        /// <summary>
        /// Same as SemanticModel.GetConstantValue().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="ISymbol"/> or null</returns>
        public static Optional<object> GetConstantValueSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            return semanticModel.SemanticModelFor(node)
                                ?.GetConstantValue(node, cancellationToken) ?? default(Optional<object>);
        }

        /// <summary>
        /// Same as SemanticModel.GetTypeInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="TypeInfo"/> or default(TypeInfo)</returns>
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

            return Cache.GetOrAdd(expression.SyntaxTree, GetSemanticModel);

            SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
            {
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
}
