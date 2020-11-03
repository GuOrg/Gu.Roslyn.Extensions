namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for getting symbols.
    /// </summary>
    public static partial class SemanticModelExt
    {
        /// <summary>
        /// Try getting the <see cref="ISymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <typeparam name="TSymbol">The symbol.</typeparam>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="token">The <see cref="SyntaxToken"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol<TSymbol>(this SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken, [NotNullWhen(true)] out TSymbol? symbol)
            where TSymbol : class, ISymbol
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (token.Parent is null)
            {
                throw new System.ArgumentNullException(nameof(token), "Token.Parent is null");
            }

            symbol = GetSymbolSafe(semanticModel, token.Parent, cancellationToken) as TSymbol ??
                     GetDeclaredSymbolSafe(semanticModel, token.Parent, cancellationToken) as TSymbol;
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ISymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <typeparam name="TSymbol">The symbol.</typeparam>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol<TSymbol>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken, [NotNullWhen(true)] out TSymbol? symbol)
            where TSymbol : class, ISymbol
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            symbol = GetSymbolSafe(semanticModel, node, cancellationToken) as TSymbol ??
                     GetDeclaredSymbolSafe(semanticModel, node, cancellationToken) as TSymbol;
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ConstructorInitializerSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ConstructorInitializerSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            symbol = GetSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ObjectCreationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ObjectCreationExpressionSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            symbol = GetSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ObjectCreationExpressionSyntax"/>.</param>
        /// <param name="expected">The expected method.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ObjectCreationExpressionSyntax node, QualifiedType expected, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (expected is null)
            {
                throw new System.ArgumentNullException(nameof(expected));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.Type is SimpleNameSyntax typeName &&
                (typeName.Identifier.ValueText == expected.Type ||
                 AliasWalker.TryGet(node.SyntaxTree, typeName.Identifier.ValueText, out _)))
            {
                symbol = semanticModel.GetSymbolSafe(node, cancellationToken);
                return symbol is { } &&
                       symbol.ContainingType == expected;
            }

            if (node.Type is QualifiedNameSyntax { Right: { } right } &&
                right.Identifier.ValueText == expected.Type)
            {
                symbol = semanticModel.GetSymbolSafe(node, cancellationToken);
                return symbol is { } &&
                       symbol.ContainingType == expected;
            }

            symbol = null;
            return false;
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, InvocationExpressionSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            symbol = GetSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The expected method.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, InvocationExpressionSyntax node, QualifiedMethod expected, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (expected is null)
            {
                throw new System.ArgumentNullException(nameof(expected));
            }

            symbol = null;

            if (node.TryGetMethodName(out var name) &&
                name != expected.Name)
            {
                return false;
            }

            if (semanticModel.TryGetSymbol(node, cancellationToken, out var candidate) &&
                candidate == expected)
            {
                symbol = candidate;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try getting the <see cref="IPropertySymbol"/> or <see cref="IArrayTypeSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ElementAccessExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ElementAccessExpressionSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            symbol = GetSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="AwaitExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static ISymbol? GetSymbolSafe(this SemanticModel semanticModel, AwaitExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return node is { Expression: { } expression }
                ? semanticModel.GetSymbolSafe(expression, cancellationToken)
                : null;
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ConstructorInitializerSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static IMethodSymbol? GetSymbolSafe(this SemanticModel semanticModel, ConstructorInitializerSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return (IMethodSymbol?)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ObjectCreationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static IMethodSymbol? GetSymbolSafe(this SemanticModel semanticModel, ObjectCreationExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return (IMethodSymbol?)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static IMethodSymbol? GetSymbolSafe(this SemanticModel semanticModel, InvocationExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return (IMethodSymbol?)semanticModel.GetSymbolSafe((SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ElementAccessExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IPropertySymbol"/> or an <see cref="IArrayTypeSymbol"/> or null.</returns>
        public static ISymbol? GetSymbolSafe(this SemanticModel semanticModel, ElementAccessExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return GetSymbolSafe(semanticModel, (SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="token"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="token">The <see cref="SyntaxToken"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static ISymbol? GetSymbolSafe(this SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (token.Parent is null)
            {
                throw new System.ArgumentNullException(nameof(token), "Token.Parent is null");
            }

            return GetSymbolSafe(semanticModel, token.Parent, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetSymbolInfo().Symbol but works when <paramref name="node"/> is not in the syntax tree.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static ISymbol? GetSymbolSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return node switch
            {
                AwaitExpressionSyntax awaitExpression => GetSymbolSafe(semanticModel, awaitExpression, cancellationToken),
                ElementAccessExpressionSyntax { Expression: { } expression }
                    when semanticModel.TryGetType(expression, cancellationToken, out var type) && type is IArrayTypeSymbol arrayType
                    => arrayType,
                _ => semanticModel.SemanticModelFor(node)?.GetSymbolInfo(node, cancellationToken).Symbol,
            };
        }
    }
}
