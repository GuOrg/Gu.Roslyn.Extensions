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

        /// <summary>
        /// Get a walker with all mutations for <paramref name="property"/>
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker with all mutations for <paramref name="property"/></returns>
        public static MutationWalker For(IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (property.ContainingType.TrySingleDeclaration(cancellationToken, out TypeDeclarationSyntax typeDeclaration))
            {
                var walker = Borrow(typeDeclaration);
                walker.mutations.RemoveAll(NotForProperty);
                return walker;
            }

            return Borrow(() => new MutationWalker());

            bool NotForProperty(SyntaxNode mutation)
            {
                switch (mutation)
                {
                    case AssignmentExpressionSyntax assignment:
                        return !IsProperty(assignment.Left);
                    case PrefixUnaryExpressionSyntax unary:
                        return !IsProperty(unary.Operand);
                    case PostfixUnaryExpressionSyntax unary:
                        return !IsProperty(unary.Operand);
                    case ArgumentSyntax _:
                        return true;
                    default:
                        return true;
                }
            }

            bool IsProperty(ExpressionSyntax expression)
            {
                return semanticModel.TryGetSymbol(expression, cancellationToken, out ISymbol symbol) &&
                       Equals(symbol, property);
            }
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
            if (field.ContainingType.TrySingleDeclaration(cancellationToken, out TypeDeclarationSyntax typeDeclaration))
            {
                var walker = Borrow(typeDeclaration);
                walker.mutations.RemoveAll(NotForField);
                return walker;
            }

            return Borrow(() => new MutationWalker());

            bool NotForField(SyntaxNode mutation)
            {
                switch (mutation)
                {
                    case AssignmentExpressionSyntax assignment:
                        return !IsField(assignment.Left);
                    case PrefixUnaryExpressionSyntax unary:
                        return !IsField(unary.Operand);
                    case PostfixUnaryExpressionSyntax unary:
                        return !IsField(unary.Operand);
                    case ArgumentSyntax argument:
                        return !IsField(argument.Expression);
                    default:
                        return true;
                }
            }

            bool IsField(ExpressionSyntax expression)
            {
                return semanticModel.TryGetSymbol(expression, cancellationToken, out ISymbol symbol) &&
                       Equals(symbol, field);
            }
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
