namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Get all mutations in the current scope.
    /// </summary>
    public sealed class MutationWalker : ExecutionWalker<MutationWalker>, IReadOnlyList<SyntaxNode>
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
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker that has visited <paramref name="node"/></returns>
        public static MutationWalker Borrow(SyntaxNode node, Scope scope, SemanticModel semanticModel, CancellationToken cancellationToken) => BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new MutationWalker());

        /// <summary>
        /// Get a walker with all mutations for <paramref name="fieldOrProperty"/>
        /// </summary>
        /// <param name="fieldOrProperty">The <see cref="FieldOrProperty"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker with all mutations for <paramref name="fieldOrProperty"/></returns>
        public static MutationWalker For(FieldOrProperty fieldOrProperty, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (fieldOrProperty.Symbol.ContainingType.TrySingleDeclaration(cancellationToken, out TypeDeclarationSyntax typeDeclaration))
            {
                var walker = Borrow(typeDeclaration, Scope.Instance, semanticModel, cancellationToken);
                walker.mutations.RemoveAll(NotForFieldOrProperty);
                return walker;
            }

            return Borrow(() => new MutationWalker());

            bool NotForFieldOrProperty(SyntaxNode mutation)
            {
                switch (mutation)
                {
                    case AssignmentExpressionSyntax assignment:
                        return !IsFieldOrProperty(assignment.Left);
                    case PrefixUnaryExpressionSyntax unary:
                        return !IsFieldOrProperty(unary.Operand);
                    case PostfixUnaryExpressionSyntax unary:
                        return !IsFieldOrProperty(unary.Operand);
                    case ArgumentSyntax argument when fieldOrProperty.Symbol is IFieldSymbol field:
                        return !IsFieldOrProperty(argument.Expression);
                    default:
                        return true;
                }
            }

            bool IsFieldOrProperty(ExpressionSyntax expression)
            {
                return semanticModel.TryGetSymbol(expression, cancellationToken, out ISymbol symbol) &&
                       symbol.Equals(fieldOrProperty.Symbol);
            }
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="property"/>
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker with all mutations for <paramref name="property"/></returns>
        public static MutationWalker For(IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (FieldOrProperty.TryCreate(property, out var fieldOrProperty))
            {
                return For(fieldOrProperty, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="field"/>
        /// </summary>
        /// <param name="field">The <see cref="IFieldSymbol"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker with all mutations for <paramref name="field"/></returns>
        public static MutationWalker For(IFieldSymbol field, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (FieldOrProperty.TryCreate(field, out var fieldOrProperty))
            {
                return For(fieldOrProperty, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

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
