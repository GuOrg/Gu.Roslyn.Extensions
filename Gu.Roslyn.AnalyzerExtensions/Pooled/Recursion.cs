namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public sealed class Recursion : IDisposable
    {
        private static readonly ConcurrentQueue<Recursion> Cache = new ConcurrentQueue<Recursion>();
        private readonly HashSet<(string, SyntaxNode)> visited = new HashSet<(string, SyntaxNode)>();

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

        public MethodDeclarationSyntax? Target(InvocationExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, node)) &&
                node.TryGetTargetDeclaration(this.SemanticModel, this.CancellationToken, out var target))
            {
                return target;
            }

            return null;
        }

        public ConstructorDeclarationSyntax? Target(ObjectCreationExpressionSyntax node, [CallerMemberName] string caller = null)
        {
            if (this.visited.Add((caller, node)) &&
                node.TryGetTargetDeclaration(this.SemanticModel, this.CancellationToken, out var target))
            {
                return target;
            }

            return null;
        }

        public T? Target<T>(ExpressionSyntax node, [CallerMemberName] string caller = null)
            where T : SyntaxNode
        {
            if (this.visited.Add((caller, node)) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var symbol) &&
                symbol.TrySingleDeclaration(this.CancellationToken, out T? declaration))
            {
                return declaration;
            }

            return null;
        }
    }
}
