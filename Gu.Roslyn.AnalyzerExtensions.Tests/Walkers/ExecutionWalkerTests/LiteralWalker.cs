namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.ExecutionWalkerTests
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class LiteralWalker : ExecutionWalker<LiteralWalker>
    {
        private readonly List<LiteralExpressionSyntax> literals = new();

        internal IReadOnlyList<LiteralExpressionSyntax> Literals => this.literals;

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                this.literals.Add(node);
            }

            base.VisitLiteralExpression(node);
        }

        internal static LiteralWalker Borrow(SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new LiteralWalker());
        }

        protected override void Clear()
        {
            this.literals.Clear();
            base.Clear();
        }
    }
}
