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

    /// <summary>
    /// A helper for walking syntax trees safely in case of recursion.
    /// The target methods are only callable once for each node.
    /// </summary>
    public sealed class Recursion : IDisposable
    {
        private static readonly ConcurrentQueue<Recursion> Cache = new ConcurrentQueue<Recursion>();
        private readonly HashSet<(string?, int, SyntaxNode)> visited = new HashSet<(string?, int, SyntaxNode)>();

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

            recursion.SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            recursion.CancellationToken = cancellationToken;
            return recursion;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Clear();
            Cache.Enqueue(this);
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,MethodDeclarationSyntax}"/>.</returns>
        public Target<IMethodSymbol, MethodDeclarationSyntax>? Target(InvocationExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out MethodDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,ConstructorDeclarationSyntax}"/>.</returns>
        public Target<IMethodSymbol, ConstructorDeclarationSyntax>? Target(ObjectCreationExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,ConstructorDeclarationSyntax}"/>.</returns>
        public Target<IMethodSymbol, ConstructorDeclarationSyntax>? Target(ConstructorInitializerSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IParameterSymbol,BaseMethodDeclarationSyntax}"/>.</returns>
        public Target<IParameterSymbol, BaseMethodDeclarationSyntax>? Target(ArgumentSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                node is { Parent: ArgumentListSyntax { Parent: { } parent } } &&
                this.SemanticModel.TryGetSymbol(parent, this.CancellationToken, out IMethodSymbol? method) &&
                method.TryFindParameter(node, out var symbol))
            {
                _ = method.TrySingleDeclaration(this.CancellationToken, out BaseMethodDeclarationSyntax? declaration);
                return Gu.Roslyn.AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IParameterSymbol,AccessorDeclarationSyntax}"/>.</returns>
        public Target<IParameterSymbol, AccessorDeclarationSyntax>? PropertySet(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out IPropertySymbol? property) &&
                property is { SetMethod: { Parameters: { Length: 1 } } set })
            {
                _ = set.TrySingleDeclaration(this.CancellationToken, out AccessorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(set.Parameters[0], declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,CSharpSyntaxNode}"/>.</returns>
        public Target<IMethodSymbol, CSharpSyntaxNode>? PropertyGet(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out IPropertySymbol? property) &&
                property is { GetMethod: { } get })
            {
                _ = get.TrySingleDeclaration(this.CancellationToken, out CSharpSyntaxNode? declaration);
                return AnalyzerExtensions.Target.Create(get, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,CSharpSyntaxNode}"/>.</returns>
        public Target<IMethodSymbol, MethodDeclarationSyntax>? MethodGroup(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out IMethodSymbol? symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out MethodDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,CSharpSyntaxNode}"/>.</returns>
        public Target<ILocalSymbol, SyntaxNode>? MethodGroup(VariableDeclaratorSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out ILocalSymbol? symbol))
            {
                _ = symbol.TryGetScope(this.CancellationToken, out var declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{INamedTypeSymbol,TypeDeclarationSyntax}"/>.</returns>
        public Target<INamedTypeSymbol, TypeDeclarationSyntax>? ContainingType(SyntaxNode node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out ISymbol? symbol) &&
                symbol.ContainingSymbol is INamedTypeSymbol type)
            {
                _ = type.TrySingleDeclaration(this.CancellationToken, out TypeDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(type, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="TSymbol">The type of symbol expected.</typeparam>
        /// <typeparam name="TDeclaration">The type of declaration expected.</typeparam>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</returns>
        public Target<TSymbol, TDeclaration>? Target<TSymbol, TDeclaration>(SyntaxNode node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
            where TSymbol : class, ISymbol
            where TDeclaration : CSharpSyntaxNode
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out TSymbol? symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out TDeclaration? declaration);
                return AnalyzerExtensions.Target.Create(symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Clear the inner set.
        /// </summary>
        public void Clear()
        {
            this.visited.Clear();
            this.SemanticModel = null!;
            this.CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Clear the inner set.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        public void Restart(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            this.visited.Clear();
            this.SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            this.CancellationToken = cancellationToken;
        }
    }
}
