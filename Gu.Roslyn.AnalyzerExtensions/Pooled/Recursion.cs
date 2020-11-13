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
        /// Gets the containing type where the recursion started.
        /// This is used for override resolution.
        /// </summary>
        public INamedTypeSymbol ContainingType { get; private set; } = null!;

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
        /// <param name="containingType">The <see cref="INamedTypeSymbol"/> where recursion starts..</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A <see cref="Recursion"/>.</returns>
        public static Recursion Borrow(INamedTypeSymbol containingType, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (containingType is null)
            {
                throw new ArgumentNullException(nameof(containingType));
            }

            if (!Cache.TryDequeue(out var recursion))
            {
                recursion = new Recursion();
            }

            recursion.ContainingType = containingType;
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
        public Target<ExpressionSyntax, ISymbol, CSharpSyntaxNode>? Target(InvocationExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.EffectiveSymbol<IMethodSymbol>(node) is { } symbol)
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out CSharpSyntaxNode? declaration);
                if (symbol is { IsExtensionMethod: true, ReducedFrom: { } reducedFrom })
                {
                    switch (node)
                    {
                        case { Expression: MemberAccessExpressionSyntax { Expression: { } expression } }:
                            return AnalyzerExtensions.Target.Create(expression, (ISymbol)reducedFrom.Parameters[0], declaration);
                        case { Expression: MemberBindingExpressionSyntax _, Parent: ConditionalAccessExpressionSyntax { Expression: { } expression } }:
                            return AnalyzerExtensions.Target.Create(expression, (ISymbol)reducedFrom.Parameters[0], declaration);
                    }
                }

                return AnalyzerExtensions.Target.Create((ExpressionSyntax)node, (ISymbol)symbol, declaration);
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
        public Target<ObjectCreationExpressionSyntax, IMethodSymbol, ConstructorDeclarationSyntax>? Target(ObjectCreationExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
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
        public Target<ConstructorInitializerSyntax, IMethodSymbol, ConstructorDeclarationSyntax>? Target(ConstructorInitializerSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
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
        public Target<ArgumentSyntax, IParameterSymbol, BaseMethodDeclarationSyntax>? Target(ArgumentSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                node is { Parent: ArgumentListSyntax { Parent: { } parent } } &&
                this.EffectiveSymbol<IMethodSymbol>(parent) is { } method &&
                method.TryFindParameter(node, out var symbol))
            {
                _ = method.TrySingleDeclaration(this.CancellationToken, out BaseMethodDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
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
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,SyntaxNode}"/>.</returns>
        public Target<VariableDeclaratorSyntax, ILocalSymbol, SyntaxNode>? Target(VariableDeclaratorSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out ILocalSymbol? symbol))
            {
                _ = symbol.TryGetScope(this.CancellationToken, out var declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
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
        public Target<TypeSyntax, ITypeSymbol, TypeDeclarationSyntax>? Target(TypeSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out ITypeSymbol? symbol))
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out TypeDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
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
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,SyntaxNode}"/>.</returns>
        public Target<ExpressionSyntax, ISymbol, SyntaxNode>? Target(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            return node switch
            {
                InvocationExpressionSyntax invocation
                when this.Target(invocation, caller, line) is { } temp
                => AnalyzerExtensions.Target.Create(temp.Source, (ISymbol)temp.Symbol, (SyntaxNode?)temp.Declaration),
                _ => this.Target<ExpressionSyntax, ISymbol, SyntaxNode>(node, caller, line),
            };
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{IParameterSymbol,AccessorDeclarationSyntax}"/>.</returns>
        public Target<ExpressionSyntax, IParameterSymbol, AccessorDeclarationSyntax>? PropertySet(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.EffectiveSymbol<IPropertySymbol>(node) is { } property &&
                property is { SetMethod: { Parameters: { Length: 1 } } set })
            {
                _ = set.TrySingleDeclaration(this.CancellationToken, out AccessorDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, set.Parameters[0], declaration);
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
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,SyntaxNode}"/>.</returns>
        public Target<ExpressionSyntax, IMethodSymbol, SyntaxNode>? PropertyGet(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.EffectiveSymbol<IPropertySymbol>(node) is { } property &&
                property is { GetMethod: { } get })
            {
                _ = get.TrySingleDeclaration(this.CancellationToken, out SyntaxNode? declaration);
                return AnalyzerExtensions.Target.Create(node, get, declaration);
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
        /// <returns>A <see cref="SymbolAndDeclaration{IMethodSymbol,SyntaxNode}"/>.</returns>
        public Target<ExpressionSyntax, IMethodSymbol, MethodDeclarationSyntax>? MethodGroup(ExpressionSyntax node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (this.visited.Add((caller, line, node)) &&
                this.EffectiveSymbol<IMethodSymbol>(node) is { } symbol)
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out MethodDeclarationSyntax? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Get the target symbol and declaration if exists.
        /// Calling this is safe in case of recursion as it only returns a value once for each called for <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="TSource">The source node.</typeparam>
        /// <typeparam name="TSymbol">The type of symbol expected.</typeparam>
        /// <typeparam name="TDeclaration">The type of declaration expected.</typeparam>
        /// <param name="node">The invocation that you want to walk the body of the declaration of if it exists.</param>
        /// <param name="caller">The invoking method.</param>
        /// <param name="line">Line number in <paramref name="caller"/>.</param>
        /// <returns>A <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</returns>
        public Target<TSource, TSymbol, TDeclaration>? Target<TSource, TSymbol, TDeclaration>(TSource node, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
            where TSource : SyntaxNode
            where TSymbol : class, ISymbol
            where TDeclaration : SyntaxNode
        {
            if (this.visited.Add((caller, line, node)) &&
                this.EffectiveSymbol<TSymbol>(node) is { } symbol)
            {
                _ = symbol.TrySingleDeclaration(this.CancellationToken, out TDeclaration? declaration);
                return AnalyzerExtensions.Target.Create(node, symbol, declaration);
            }

            return null;
        }

        /// <summary>
        /// Clear the inner set.
        /// </summary>
        public void Clear()
        {
            this.visited.Clear();
            this.ContainingType = null!;
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

        private TSymbol? EffectiveSymbol<TSymbol>(SyntaxNode node)
            where TSymbol : class, ISymbol
        {
            if (this.SemanticModel.TryGetSymbol<TSymbol>(node, this.CancellationToken, out var symbol))
            {
                symbol = (TSymbol)symbol.OriginalDefinition;
                if (IsExplicitBase())
                {
                    return symbol;
                }

                return symbol switch
                {
                    IEventSymbol @event
                       when (@event.IsVirtual || @event.IsAbstract) &&
                            this.ContainingType.FindOverride(@event) is TSymbol overrider
                        => (TSymbol)overrider.OriginalDefinition,
                    IPropertySymbol property
                        when (property.IsVirtual || property.IsAbstract) &&
                             this.ContainingType.FindOverride(property) is TSymbol overrider
                        => (TSymbol)overrider.OriginalDefinition,
                    IMethodSymbol method
                        when (method.IsVirtual || method.IsAbstract) &&
                             this.ContainingType.FindOverride(method) is TSymbol overrider
                        => (TSymbol)overrider.OriginalDefinition,
                    _ => symbol,
                };
            }

            return null;

            bool IsExplicitBase()
            {
                return node switch
                {
                    InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Expression: BaseExpressionSyntax _ } } => true,
                    MemberAccessExpressionSyntax { Expression: BaseExpressionSyntax _ } => true,
                    IdentifierNameSyntax { Parent: MemberAccessExpressionSyntax { Expression: BaseExpressionSyntax _ } } => true,
                    _ => false,
                };
            }
        }
    }
}
