namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public sealed class Recursion : IDisposable
    {
        private static readonly ConcurrentQueue<Recursion> Cache = new ConcurrentQueue<Recursion>();
        private readonly HashSet<(string, string, SyntaxNode)> visited = new HashSet<(string, string, SyntaxNode)>();

        private Recursion()
        {
        }

        /// <summary>
        /// Gets the <see cref="SemanticModel"/>.
        /// </summary>
        public SemanticModel SemanticModel { get; private set; } = null!;

        /// <summary>
        /// Gets the <see cref="CancellationToken"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Get and instance from cache, dispose returns it.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A <see cref="Recursion"/>.</returns>
        public static Recursion Borrow(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (!Cache.TryDequeue(out var recursion))
            {
                recursion = new Recursion();
            }

            recursion.SemanticModel = semanticModel;
            recursion.CancellationToken = cancellationToken;
            return recursion;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.visited.Clear();
            this.SemanticModel = null!;
            this.CancellationToken = CancellationToken.None;
        }

        public SymbolAndDeclaration<IMethodSymbol, MethodDeclarationSyntax>? Target(InvocationExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(MethodDeclarationSyntax), node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol) &&
                SymbolAndDeclaration.TryCreate(symbol, this.CancellationToken, out SymbolAndDeclaration<IMethodSymbol, MethodDeclarationSyntax> symbolAndDeclaration))
            {
                return symbolAndDeclaration;
            }

            return null;
        }

        public SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax>? Target(ObjectCreationExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(ConstructorDeclarationSyntax), node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol) &&
                SymbolAndDeclaration.TryCreate(symbol, this.CancellationToken, out SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax> symbolAndDeclaration))
            {
                return symbolAndDeclaration;
            }

            return null;
        }

        public SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax>? Target(ConstructorInitializerSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(ConstructorDeclarationSyntax), node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol) &&
                SymbolAndDeclaration.TryCreate(symbol, this.CancellationToken, out SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax> symbolAndDeclaration))
            {
                return symbolAndDeclaration;
            }

            return null;
        }

        public SymbolAndDeclaration<IParameterSymbol, BaseMethodDeclarationSyntax>? Target(ArgumentSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(BaseMethodDeclarationSyntax), node)) &&
                node is { Parent: ArgumentListSyntax { Parent: { } parent } } &&
                this.SemanticModel.TryGetSymbol(parent, this.CancellationToken, out IMethodSymbol? method) &&
                method.TryFindParameter(node, out var symbol) &&
                method.TrySingleDeclaration(this.CancellationToken, out BaseMethodDeclarationSyntax? declaration))
            {
                return new SymbolAndDeclaration<IParameterSymbol, BaseMethodDeclarationSyntax>(symbol, declaration);
            }

            return null;
        }

        public SymbolAndDeclaration<IParameterSymbol, AccessorDeclarationSyntax>? PropertySet(ExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(this.PropertySet), node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out IPropertySymbol? property) &&
                property is { SetMethod: { Parameters: { Length: 1 } } set } &&
                set.TrySingleAccessorDeclaration(this.CancellationToken, out var declaration))
            {
                return new SymbolAndDeclaration<IParameterSymbol, AccessorDeclarationSyntax>(set.Parameters[0], declaration);
            }

            return null;
        }

        public SymbolAndDeclaration<IMethodSymbol, CSharpSyntaxNode>? PropertyGet(ExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, nameof(this.PropertyGet), node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out IPropertySymbol? property) &&
                property is { GetMethod: { } get } &&
                get.TrySingleDeclaration(this.CancellationToken, out CSharpSyntaxNode? declaration))
            {
                return new SymbolAndDeclaration<IMethodSymbol, CSharpSyntaxNode>(get, declaration);
            }

            return null;
        }

        public SymbolAndDeclaration<TSymbol, TDeclaration>? Target<TSymbol, TDeclaration>(ExpressionSyntax node, [CallerMemberName] string caller = null)
            where TSymbol : class, ISymbol
            where TDeclaration : CSharpSyntaxNode
        {
            if (this.visited.Add((caller, typeof(TSymbol).Name + typeof(TDeclaration).Name, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out TSymbol? symbol) &&
                symbol.TrySingleDeclaration(this.CancellationToken, out TDeclaration? declaration))
            {
                return new SymbolAndDeclaration<TSymbol, TDeclaration>(symbol, declaration);
            }

            return null;
        }
    }
}
