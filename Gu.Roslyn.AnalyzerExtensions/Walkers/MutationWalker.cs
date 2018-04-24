namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Get all mutations in the current scope.
    /// </summary>
    public sealed class MutationWalker : PooledWalker<MutationWalker>, IReadOnlyList<SyntaxNode>
    {
        private readonly List<SyntaxNode> mutations = new List<SyntaxNode>();

        private MutationWalker()
        {
        }

        /// <inheritdoc />
        public int Count => this.mutations.Count;

        /// <inheritdoc />
        public SyntaxNode this[int index] => this.mutations[index];

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>
        /// </summary>
        /// <param name="node">The scope</param>
        /// <returns>A walker that has visited <paramref name="node"/></returns>
        public static MutationWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new MutationWalker());

        /// <inheritdoc />
        public IEnumerator<SyntaxNode> GetEnumerator() => this.mutations.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            this.mutations.Add(node);
            base.VisitAssignmentExpression(node);
        }

        /// <inheritdoc />
        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LogicalNotExpression:
                    break;
                default:
                    this.mutations.Add(node);
                    break;
            }

            base.VisitPrefixUnaryExpression(node);
        }

        /// <inheritdoc />
        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            this.mutations.Add(node);
            base.VisitPostfixUnaryExpression(node);
        }

        /// <inheritdoc />
        public override void VisitArgument(ArgumentSyntax node)
        {
            if (node.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) ||
                node.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
            {
                this.mutations.Add(node);
            }

            base.VisitArgument(node);
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.mutations.Clear();
        }
    }
}
