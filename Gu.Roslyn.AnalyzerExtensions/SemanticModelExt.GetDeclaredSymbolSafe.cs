namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for getting declared symbol when not sure it is in the syntax tree of the semantic model.
    /// </summary>
    public static partial class SemanticModelExt
    {
        /// <summary>
        /// Try getting the <see cref="ITypeSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, TypeDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ITypeSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IFieldSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, FieldDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IFieldSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ConstructorDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ConstructorDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IEventSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, EventDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IEventSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IEventSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, EventFieldDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IEventSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IPropertySymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, PropertyDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IPropertySymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IPropertySymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="IndexerDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, IndexerDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IPropertySymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IMethodSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, MethodDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="IParameterSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ParameterSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ParameterSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out IParameterSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The <see cref="ILocalSymbol"/> or <see cref="IFieldSymbol"/> if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, VariableDeclarationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDeclaratorSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The <see cref="ILocalSymbol"/> or <see cref="IFieldSymbol"/> if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, VariableDeclaratorSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DeclarationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found. Can be <see cref="IDiscardSymbol"/>.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, DeclarationExpressionSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DeclarationPatternSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found. Can be <see cref="IDiscardSymbol"/>.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, DeclarationPatternSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="SingleVariableDesignationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, SingleVariableDesignationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ILocalSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDesignationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found. Can be <see cref="IDiscardSymbol"/>.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, VariableDesignationSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DiscardDesignationSyntax"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, DiscardDesignationSyntax node, [NotNullWhen(true)] out IDiscardSymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node);
            return symbol is { };
        }

        /// <summary>
        /// Try getting the <see cref="ILocalSymbol"/> for the node.
        /// Gets the semantic model for the tree if the node is not in the tree corresponding to <paramref name="semanticModel"/>.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="symbol">The symbol if found.</param>
        /// <returns>True if a symbol was found.</returns>
        public static bool TryGetSymbol(this SemanticModel semanticModel, ExpressionSyntax node, CancellationToken cancellationToken, [NotNullWhen(true)] out ISymbol? symbol)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            symbol = GetDeclaredSymbolSafe(semanticModel, node, cancellationToken) ??
                     GetSymbolSafe(semanticModel, node, cancellationToken);
            return symbol is { };
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ITypeSymbol"/> or null.</returns>
        public static ITypeSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, TypeDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IFieldSymbol"/> or null.</returns>
        public static IFieldSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, FieldDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Declaration is { Variables: { } variables } &&
                variables.TrySingle(out var variable))
            {
                return (IFieldSymbol?)semanticModel.SemanticModelFor(node)
                                                  ?.GetDeclaredSymbol(variable, cancellationToken);
            }

            return null;
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ConstructorDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IMethodSymbol"/> or null.</returns>
        public static IMethodSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, ConstructorDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IEventSymbol"/> or null.</returns>
        public static IEventSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, EventDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IEventSymbol"/> or null.</returns>
        public static IEventSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, EventFieldDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Declaration is { Variables: { } variables } &&
                variables.TrySingle(out var variable))
            {
                return semanticModel.SemanticModelFor(node)
                                   ?.GetDeclaredSymbol(variable, cancellationToken) as IEventSymbol;
            }

            return null;
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IPropertySymbol"/> or null.</returns>
        public static IPropertySymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, PropertyDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="IndexerDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IPropertySymbol"/> or null.</returns>
        public static IPropertySymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, IndexerDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, BasePropertyDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IMethodSymbol"/> or null.</returns>
        public static IMethodSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, MethodDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="ParameterSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IParameterSymbol"/> or null.</returns>
        public static IParameterSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, ParameterSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return (IParameterSymbol?)GetDeclaredSymbolSafe(semanticModel, (SyntaxNode)node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDeclarationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ILocalSymbol"/> or <see cref="IFieldSymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, VariableDeclarationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Variables.TrySingle(out var variable))
            {
                return semanticModel.SemanticModelFor(node)
                                   ?.GetDeclaredSymbol(variable, cancellationToken);
            }

            return null;
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDeclaratorSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ILocalSymbol"/> or <see cref="IFieldSymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, VariableDeclaratorSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return semanticModel.SemanticModelFor(node)
                               ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DeclarationExpressionSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ILocalSymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, DeclarationExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return GetDeclaredSymbolSafe(semanticModel, node.Designation, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DeclarationPatternSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ILocalSymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, DeclarationPatternSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return GetDeclaredSymbolSafe(semanticModel, node.Designation, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="SingleVariableDesignationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ILocalSymbol"/> or null.</returns>
        public static ILocalSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, SingleVariableDesignationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return (ILocalSymbol?)semanticModel.SemanticModelFor(node)
                                             ?.GetDeclaredSymbol(node, cancellationToken);
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="VariableDesignationSyntax"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or <see cref="IDiscardSymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, VariableDesignationSyntax node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node switch
            {
                SingleVariableDesignationSyntax singleVariable => GetDeclaredSymbolSafe(semanticModel, singleVariable, cancellationToken),
                DiscardDesignationSyntax discard => GetDeclaredSymbolSafe(semanticModel, discard),
                _ => (ISymbol?)null,
            };
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="DiscardDesignationSyntax"/>.</param>
        /// <returns>An <see cref="IDiscardSymbol"/> or null.</returns>
        public static IDiscardSymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, DiscardDesignationSyntax node)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Working around https://github.com/dotnet/roslyn/issues/34031
            // This is not pretty.
            return node.Parent switch
            {
                DeclarationExpressionSyntax declarationExpression => semanticModel.SemanticModelFor(node)
                                                                                  ?.GetSpeculativeSymbolInfo(node.SpanStart, declarationExpression, SpeculativeBindingOption.BindAsExpression).Symbol as IDiscardSymbol,
                DeclarationPatternSyntax { Type: { } type } => semanticModel.SemanticModelFor(node)
                                                                           ?.GetSpeculativeSymbolInfo(node.SpanStart, SyntaxFactory.DeclarationExpression(type, node), SpeculativeBindingOption.BindAsExpression).Symbol as IDiscardSymbol,

                _ => null,
            };
        }

        /// <summary>
        /// Same as SemanticModel.GetDeclaredSymbol but works when <paramref name="node"/> is not in the syntax tree.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="ISymbol"/> or null.</returns>
        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node switch
            {
                FieldDeclarationSyntax fieldDeclaration => GetDeclaredSymbolSafe(semanticModel, fieldDeclaration, cancellationToken),
                DeclarationExpressionSyntax declaration => GetDeclaredSymbolSafe(semanticModel, declaration, cancellationToken),
                DiscardDesignationSyntax discardDesignation => GetDeclaredSymbolSafe(semanticModel, discardDesignation, cancellationToken),
                VariableDeclarationSyntax variableDeclaration => GetDeclaredSymbolSafe(semanticModel, variableDeclaration, cancellationToken),
                _ => semanticModel.SemanticModelFor(node)
                                 ?.GetDeclaredSymbol(node, cancellationToken),
            };
        }
    }
}
