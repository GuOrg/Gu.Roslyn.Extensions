namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A walker that finds assignments in given scope.
    /// </summary>
    public sealed class AssignmentExecutionWalker : ExecutionWalker<AssignmentExecutionWalker>
    {
        private readonly List<AssignmentExpressionSyntax> assignments = new List<AssignmentExpressionSyntax>();
        private readonly List<ArgumentSyntax> arguments = new List<ArgumentSyntax>();
        private readonly List<LocalDeclarationStatementSyntax> localDeclarations = new List<LocalDeclarationStatementSyntax>();

        private AssignmentExecutionWalker()
        {
        }

        /// <summary>
        /// Gets a list with all <see cref="AssignmentExpressionSyntax"/> in the scope.
        /// </summary>
        public IReadOnlyList<AssignmentExpressionSyntax> Assignments => this.assignments;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static AssignmentExecutionWalker Borrow(SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new AssignmentExecutionWalker());
        }

        /// <summary>
        /// Get all <see cref="AssignmentExpressionSyntax"/> for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to find assignments for.</param>
        /// <param name="node">The node to walk.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if an assignment was found for the symbol.</returns>
        public static AssignmentExecutionWalker For(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            var walker = Borrow(node, scope, semanticModel, cancellationToken);
            walker.assignments.RemoveAll(x => !IsMatch(symbol.OriginalDefinition, x.Left, semanticModel, cancellationToken));
            return walker;
        }

        /// <summary>
        /// Get all <see cref="AssignmentExpressionSyntax"/> with <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to find assignments for.</param>
        /// <param name="node">The node to walk.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if an assignment was found for the symbol.</returns>
        public static AssignmentExecutionWalker With(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return With(symbol.OriginalDefinition, node);

            AssignmentExecutionWalker With(ISymbol currentSymbol, SyntaxNode currentNode, Recursion? recursion = null)
            {
                var walker = Borrow(currentNode, SearchScope.Member, semanticModel, cancellationToken);
                walker.assignments.RemoveAll(x => !IsMatch(currentSymbol, x.Right, semanticModel, cancellationToken));

                foreach (var declaration in walker.localDeclarations)
                {
                    if (declaration.Declaration is { Variables: { } variables } &&
                        variables.TryFirst(x => x.Initializer != null, out var variable) &&
                        IsMatch(currentSymbol, variable.Initializer.Value, semanticModel, cancellationToken) &&
                        semanticModel.TryGetSymbol(variable, cancellationToken, out ILocalSymbol? local))
                    {
                        using (var localWalker = With(local, currentNode, recursion))
                        {
                            walker.assignments.AddRange(localWalker.Assignments);
                        }
                    }
                }

                if (scope != SearchScope.Member)
                {
                    using (recursion = Recursion.Borrow(semanticModel, cancellationToken))
                    {
                        foreach (var argument in walker.arguments)
                        {
                            if (argument is { Expression: IdentifierNameSyntax { Identifier: { ValueText: { } name } }, Parent: ArgumentListSyntax { Parent: { } } } &&
                                name == currentSymbol.Name &&
                                recursion.Target(argument) is { Symbol: { } parameter, Declaration: { } target })
                            {
                                using (var invocationWalker = With(parameter, target, recursion))
                                {
                                    walker.assignments.AddRange(invocationWalker.Assignments);
                                }
                            }
                        }

                        while (TryWalkBackingField(out var assignment, out var setterWalker))
                        {
                            walker.assignments.Remove(assignment);
                            walker.assignments.AddRange(setterWalker.assignments);
                        }

                        bool TryWalkBackingField(out AssignmentExpressionSyntax propertyAssignment, out AssignmentExecutionWalker setterWalker)
                        {
                            propertyAssignment = null!;
                            setterWalker = null!;

                            foreach (var candidate in walker.assignments)
                            {
                                if (candidate is { Left: { } left } &&
                                    recursion!.PropertySet(left) is { Symbol: { } value, Declaration: { } setter })
                                {
                                    propertyAssignment = candidate;
                                    setterWalker = With(value, setter, recursion);
                                    return true;
                                }
                            }

                            return false;
                        }
                    }
                }

                return walker;
            }
        }

        /// <summary>
        /// Get the first <see cref="AssignmentExpressionSyntax"/> for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to find assignments for.</param>
        /// <param name="node">The node to walk.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="assignment">The first assignment if any.</param>
        /// <returns>True if an assignment was found for the symbol.</returns>
        public static bool FirstFor(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out AssignmentExpressionSyntax? assignment)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            assignment = null;
            using (var walker = Borrow(node, scope, semanticModel, cancellationToken))
            {
                foreach (var candidate in walker.Assignments)
                {
                    if (semanticModel.TryGetSymbol(candidate.Left, cancellationToken, out ISymbol? assignedSymbol) &&
                        SymbolComparer.Equals(symbol.OriginalDefinition, assignedSymbol))
                    {
                        assignment = candidate;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the only <see cref="AssignmentExpressionSyntax"/> for <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to find assignments for.</param>
        /// <param name="node">The node to walk.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="assignment">The single assignment.</param>
        /// <returns>True if a single assignment was found for the symbol.</returns>
        public static bool SingleFor(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out AssignmentExpressionSyntax? assignment)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            assignment = null;
            using (var walker = Borrow(node, scope, semanticModel, cancellationToken))
            {
                foreach (var candidate in walker.Assignments)
                {
                    if (semanticModel.TryGetSymbol(candidate.Left, cancellationToken, out var assignedSymbol) &&
                        SymbolComparer.Equals(symbol.OriginalDefinition, assignedSymbol))
                    {
                        if (assignment != null)
                        {
                            assignment = null;
                            return false;
                        }

                        assignment = candidate;
                    }
                }
            }

            return assignment != null;
        }

        /// <summary>
        /// Get the first <see cref="AssignmentExpressionSyntax"/> with <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to find assignments for.</param>
        /// <param name="node">The node to walk.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="assignment">The single assignment.</param>
        /// <returns>True if a single assignment was found for the symbol.</returns>
        public static bool FirstWith(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out AssignmentExpressionSyntax? assignment)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            using (var walker = With(symbol.OriginalDefinition, node, scope, semanticModel, cancellationToken))
            {
                return walker.assignments.TryFirst(out assignment);
            }
        }

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            this.assignments.Add(node);
            base.VisitAssignmentExpression(node);
        }

        /// <inheritdoc />
        public override void VisitArgument(ArgumentSyntax node)
        {
            this.arguments.Add(node);
            base.VisitArgument(node);
        }

        /// <inheritdoc />
        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            this.localDeclarations.Add(node);
            base.VisitLocalDeclarationStatement(node);
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.assignments.Clear();
            this.arguments.Clear();
            this.localDeclarations.Clear();
            base.Clear();
        }

        private static bool IsMatch(ISymbol symbol, ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (expression)
            {
                case ConditionalExpressionSyntax { WhenTrue: { } whenTrue, WhenFalse: { } whenFalse }:
                    return IsMatch(symbol, whenTrue, semanticModel, cancellationToken) ||
                           IsMatch(symbol, whenFalse, semanticModel, cancellationToken);
                case BinaryExpressionSyntax { Left: { } left, Right: { } right } binary
                    when binary.IsKind(SyntaxKind.CoalesceExpression):
                    return IsMatch(symbol, left, semanticModel, cancellationToken) ||
                           IsMatch(symbol, right, semanticModel, cancellationToken);
                case BinaryExpressionSyntax { Left: { } left } binary
                    when binary.IsKind(SyntaxKind.AsExpression):
                    return IsMatch(symbol, left, semanticModel, cancellationToken);
                case CastExpressionSyntax { Expression: { } castee }:
                    return IsMatch(symbol, castee, semanticModel, cancellationToken);
                case ObjectCreationExpressionSyntax { ArgumentList: { Arguments: { } arguments } }:
                    foreach (var argument in arguments)
                    {
                        if (IsMatch(symbol, argument.Expression, semanticModel, cancellationToken))
                        {
                            return true;
                        }
                    }

                    return false;
                default:
                    if (symbol.IsEither<ILocalSymbol, IParameterSymbol>())
                    {
                        return expression is IdentifierNameSyntax identifierName &&
                               identifierName.IsSymbol(symbol, semanticModel, cancellationToken);
                    }

                    return semanticModel.TryGetSymbol(expression, cancellationToken, out var candidateSymbol) &&
                           SymbolComparer.Equals(symbol, candidateSymbol);
            }
        }
    }
}
