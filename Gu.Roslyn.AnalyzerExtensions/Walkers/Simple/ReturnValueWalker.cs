namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="ExpressionSyntax"/> returned in the scope.
    /// </summary>
    public sealed class ReturnValueWalker : PooledWalker<ReturnValueWalker>
    {
        private readonly List<ExpressionSyntax> returnValues = new List<ExpressionSyntax>();
        private bool isSubNode;

        private ReturnValueWalker()
        {
        }

        /// <summary>
        /// Gets all <see cref="ExpressionSyntax"/> returned in the scope.
        /// </summary>
        public IReadOnlyList<ExpressionSyntax> ReturnValues => this.returnValues;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static ReturnValueWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new ReturnValueWalker());

        /// <summary>
        /// Get the single return expression.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <param name="returnValue">The <see cref="ExpressionSyntax"/>.</param>
        /// <returns>True if exactly one return expression was found.</returns>
        public static bool TrySingle(SyntaxNode node, [NotNullWhen(true)] out ExpressionSyntax? returnValue)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            using var walker = BorrowAndVisit(node, () => new ReturnValueWalker());
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return walker.returnValues.TrySingle(out returnValue);
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }

        /// <inheritdoc/>
        public override void Visit(SyntaxNode node)
        {
            if (this.isSubNode)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.LocalFunctionStatement:
                        return;
                }
            }

            this.isSubNode = true;
            base.Visit(node);
        }

        /// <inheritdoc/>
        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (node?.Expression is { } expression)
            {
                this.returnValues.Add(expression);
            }
        }

        /// <inheritdoc/>
        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            if (node?.Expression is { } expression)
            {
                this.returnValues.Add(expression);
            }
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.returnValues.Clear();
            this.isSubNode = false;
        }
    }
}
