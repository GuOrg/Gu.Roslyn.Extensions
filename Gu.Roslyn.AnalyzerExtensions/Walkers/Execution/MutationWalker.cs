namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Get all mutations in the current scope.
    /// </summary>
    public sealed class MutationWalker : ExecutionWalker<MutationWalker>
    {
        private readonly List<AssignmentExpressionSyntax> assignments = new List<AssignmentExpressionSyntax>();
        private readonly List<PrefixUnaryExpressionSyntax> prefixUnaries = new List<PrefixUnaryExpressionSyntax>();
        private readonly List<PostfixUnaryExpressionSyntax> postfixUnaries = new List<PostfixUnaryExpressionSyntax>();
        private readonly List<ArgumentSyntax> refOrOutArguments = new List<ArgumentSyntax>();

        private MutationWalker()
        {
        }

        /// <summary>
        /// Gets a list with all <see cref="AssignmentExpressionSyntax"/> found in the scope.
        /// </summary>
        public IReadOnlyList<AssignmentExpressionSyntax> Assignments => this.assignments;

        /// <summary>
        /// Gets a list with all <see cref="PrefixUnaryExpressionSyntax"/> found in the scope.
        /// </summary>
        public IReadOnlyList<PrefixUnaryExpressionSyntax> PrefixUnaries => this.prefixUnaries;

        /// <summary>
        /// Gets a list with all <see cref="PostfixUnaryExpressionSyntax"/> found in the scope.
        /// </summary>
        public IReadOnlyList<PostfixUnaryExpressionSyntax> PostfixUnaries => this.postfixUnaries;

        /// <summary>
        /// Gets a list with all <see cref="ArgumentSyntax"/> found in the scope.
        /// </summary>
        public IReadOnlyList<ArgumentSyntax> RefOrOutArguments => this.refOrOutArguments;

        /// <summary>
        /// Gets a value indicating whether there were no mutations in the scope.
        /// </summary>
        public bool IsEmpty => this.assignments.Count == 0 &&
                               this.prefixUnaries.Count == 0 &&
                               this.postfixUnaries.Count == 0 &&
                               this.refOrOutArguments.Count == 0;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static MutationWalker Borrow(SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken) => BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new MutationWalker());

        /// <summary>
        /// Get a walker with all mutations for <paramref name="fieldOrProperty"/>.
        /// </summary>
        /// <param name="fieldOrProperty">The <see cref="FieldOrProperty"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="fieldOrProperty"/>.</returns>
        public static MutationWalker For(FieldOrProperty fieldOrProperty, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (fieldOrProperty.Symbol.ContainingType.TrySingleDeclaration(cancellationToken, out TypeDeclarationSyntax? typeDeclaration))
            {
                var walker = Borrow(typeDeclaration, SearchScope.Instance, semanticModel, cancellationToken);
                walker.assignments.RemoveAll(x => !IsFieldOrProperty(x.Left));
                walker.prefixUnaries.RemoveAll(x => !IsFieldOrProperty(x.Operand));
                walker.postfixUnaries.RemoveAll(x => !IsFieldOrProperty(x.Operand));
                walker.refOrOutArguments.RemoveAll(x => !IsFieldOrProperty(x.Expression));
                return walker;
            }

            return Borrow(() => new MutationWalker());

            bool IsFieldOrProperty(ExpressionSyntax expression)
            {
                return semanticModel.TryGetSymbol(expression, cancellationToken, out ISymbol? symbol) &&
                       symbol.Equals(fieldOrProperty.Symbol);
            }
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="property"/>.</returns>
        public static MutationWalker For(IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (FieldOrProperty.TryCreate(property, out var fieldOrProperty))
            {
                return For(fieldOrProperty, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="field"/>.
        /// </summary>
        /// <param name="field">The <see cref="IFieldSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="field"/>.</returns>
        public static MutationWalker For(IFieldSymbol field, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (FieldOrProperty.TryCreate(field, out var fieldOrProperty))
            {
                return For(fieldOrProperty, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="localOrParameter"/>.
        /// </summary>
        /// <param name="localOrParameter">The <see cref="LocalOrParameter"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="localOrParameter"/>.</returns>
        public static MutationWalker For(LocalOrParameter localOrParameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (localOrParameter.TryGetScope(cancellationToken, out var node))
            {
                var walker = Borrow(node, SearchScope.Member, semanticModel, cancellationToken);
                walker.assignments.RemoveAll(x => !IsMatch(x.Left));
                walker.prefixUnaries.RemoveAll(x => !IsMatch(x.Operand));
                walker.postfixUnaries.RemoveAll(x => !IsMatch(x.Operand));
                walker.refOrOutArguments.RemoveAll(x => !IsMatch(x.Expression));
                return walker;
            }

            return Borrow(() => new MutationWalker());

            bool IsMatch(ExpressionSyntax expression)
            {
                return semanticModel.TryGetSymbol(expression, cancellationToken, out ISymbol? symbol) &&
                       symbol.Equals(localOrParameter.Symbol);
            }
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="local"/>.
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="local"/>.</returns>
        public static MutationWalker For(ILocalSymbol local, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (LocalOrParameter.TryCreate(local, out var localOrParameter))
            {
                return For(localOrParameter, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="parameter"/>.</returns>
        public static MutationWalker For(IParameterSymbol parameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (LocalOrParameter.TryCreate(parameter, out var localOrParameter))
            {
                return For(localOrParameter, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <summary>
        /// Get a walker with all mutations for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker with all mutations for <paramref name="symbol"/>.</returns>
        public static MutationWalker For(ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (LocalOrParameter.TryCreate(symbol, out var localOrParameter))
            {
                return For(localOrParameter, semanticModel, cancellationToken);
            }

            if (FieldOrProperty.TryCreate(symbol, out var fieldOrProperty))
            {
                return For(fieldOrProperty, semanticModel, cancellationToken);
            }

            return Borrow(() => new MutationWalker());
        }

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            this.assignments.Add(node);
            base.VisitAssignmentExpression(node);
        }

        /// <inheritdoc />
        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.LogicalNotExpression:
                    break;
                default:
                    this.prefixUnaries.Add(node);
                    break;
            }

            base.VisitPrefixUnaryExpression(node);
        }

        /// <inheritdoc />
        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            this.postfixUnaries.Add(node);
            base.VisitPostfixUnaryExpression(node);
        }

        /// <inheritdoc />
        public override void VisitArgument(ArgumentSyntax node)
        {
            if (node.RefOrOutKeyword.IsEither(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
            {
                this.refOrOutArguments.Add(node);
            }

            base.VisitArgument(node);
        }

        /// <summary>
        /// Try getting the single mutation found in the scope.
        /// </summary>
        /// <param name="mutation">The mutation.</param>
        /// <returns>True if exactly one mutation was found.</returns>
        public bool TrySingle([NotNullWhen(true)]out SyntaxNode? mutation)
        {
            if (this.assignments.Count == 1 &&
                this.prefixUnaries.Count == 0 &&
                this.postfixUnaries.Count == 0 &&
                this.refOrOutArguments.Count == 0)
            {
                mutation = this.assignments[0];
                return true;
            }

            if (this.assignments.Count == 0 &&
                this.prefixUnaries.Count == 1 &&
                this.postfixUnaries.Count == 0 &&
                this.refOrOutArguments.Count == 0)
            {
                mutation = this.prefixUnaries[0];
                return true;
            }

            if (this.assignments.Count == 0 &&
                this.prefixUnaries.Count == 0 &&
                this.postfixUnaries.Count == 1 &&
                this.refOrOutArguments.Count == 0)
            {
                mutation = this.postfixUnaries[0];
                return true;
            }

            if (this.assignments.Count == 0 &&
                this.prefixUnaries.Count == 0 &&
                this.postfixUnaries.Count == 0 &&
                this.refOrOutArguments.Count == 1)
            {
                mutation = this.refOrOutArguments[0];
                return true;
            }

            mutation = null;
            return false;
        }

        /// <summary>
        /// Get all mutations found in the scope.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SyntaxNode}"/>.</returns>
        public IEnumerable<SyntaxNode> All()
        {
            foreach (var assignment in this.assignments)
            {
                yield return assignment;
            }

            foreach (var prefixUnary in this.prefixUnaries)
            {
                yield return prefixUnary;
            }

            foreach (var postfixUnary in this.postfixUnaries)
            {
                yield return postfixUnary;
            }

            foreach (var argument in this.refOrOutArguments)
            {
                yield return argument;
            }
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.assignments.Clear();
            this.prefixUnaries.Clear();
            this.postfixUnaries.Clear();
            this.refOrOutArguments.Clear();
            base.Clear();
        }
    }
}
