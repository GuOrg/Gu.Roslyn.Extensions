#pragma warning disable CA1062 // Validate arguments of public methods
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class NullCheckWalker : PooledWalker<NullCheckWalker>
    {
        private readonly List<BinaryExpressionSyntax> binaryExpressions = new List<BinaryExpressionSyntax>();
        private readonly List<IsPatternExpressionSyntax> isPatterns = new List<IsPatternExpressionSyntax>();
        private readonly List<InvocationExpressionSyntax> invocations = new List<InvocationExpressionSyntax>();

        private NullCheckWalker()
        {
        }

        /// <summary>
        /// <see cref="PooledWalker{T}.Borrow"/>.
        /// </summary>
        /// <param name="scope">The scope to check.</param>
        /// <returns>A walker.</returns>
        public static NullCheckWalker Borrow(SyntaxNode scope) => BorrowAndVisit(scope, () => new NullCheckWalker());

        /// <inheritdoc />
        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (NullCheck.IsNullCheck(node, null, default, out _))
            {
                this.binaryExpressions.Add(node);
            }

            base.VisitBinaryExpression(node);
        }

        /// <inheritdoc />
        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            if (NullCheck.IsNullCheck(node, default, default, out _))
            {
                this.isPatterns.Add(node);
            }

            base.VisitIsPatternExpression(node);
        }

        /// <inheritdoc />
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (NullCheck.IsNullCheck(node, default, default, out _))
            {
                this.invocations.Add(node);
            }

            base.VisitInvocationExpression(node);
        }

        /// <summary>
        /// Try get the first null check for <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="check">The null check.</param>
        /// <returns>True if a null check was found.</returns>
        public bool TryGetFirst(IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax check)
        {
            foreach (var binaryExpression in this.binaryExpressions)
            {
                if (Is(binaryExpression.Left, parameter, semanticModel, cancellationToken) ||
                    Is(binaryExpression.Right, parameter, semanticModel, cancellationToken))
                {
                    check = binaryExpression;
                    return true;
                }
            }

            foreach (var isPattern in this.isPatterns)
            {
                if (Is(isPattern.Expression, parameter, semanticModel, cancellationToken))
                {
                    check = isPattern;
                    return true;
                }
            }

            foreach (var invocation in this.invocations)
            {
                if (invocation.ArgumentList.Arguments.TryFirst(x => Is(x.Expression, parameter, semanticModel, cancellationToken), out _))
                {
                    check = invocation;
                    return true;
                }
            }

            check = null;
            return false;
        }

        /// <summary>
        /// Filter null checks to be for <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        internal void Filter(IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            for (var i = this.binaryExpressions.Count - 1; i >= 0; i--)
            {
                var binary = this.binaryExpressions[i];
                if (Is(binary.Left, parameter, semanticModel, cancellationToken) ||
                    Is(binary.Right, parameter, semanticModel, cancellationToken))
                {
                    this.binaryExpressions.RemoveAt(i);
                }
            }

            for (var i = this.isPatterns.Count - 1; i >= 0; i--)
            {
                if (Is(this.isPatterns[i], parameter, semanticModel, cancellationToken))
                {
                    this.isPatterns.RemoveAt(i);
                }
            }

            for (var i = this.invocations.Count - 1; i >= 0; i--)
            {
                var invocation = this.invocations[i];
                if (!invocation.ArgumentList.Arguments.TryFirst(x => Is(x.Expression, parameter, semanticModel, cancellationToken), out _))
                {
                    this.invocations.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.binaryExpressions.Clear();
            this.isPatterns.Clear();
            this.invocations.Clear();
        }

        private static bool Is(ExpressionSyntax expression, IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return expression is IdentifierNameSyntax identifier &&
                   identifier.IsSymbol(parameter, semanticModel, cancellationToken);
        }
    }
}
