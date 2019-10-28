namespace Gu.Roslyn.AnalyzerExtensions
{
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
            if (symbol is null ||
                node is null)
            {
                return Borrow(() => new AssignmentExecutionWalker());
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
            if (symbol is null ||
                node is null)
            {
                return Borrow(() => new AssignmentExecutionWalker());
            }

            return With(symbol.OriginalDefinition, node, null);

            AssignmentExecutionWalker With(ISymbol currentSymbol, SyntaxNode currentNode, PooledSet<ISymbol> visited = null)
            {
                var walker = Borrow(currentNode, SearchScope.Member, semanticModel, cancellationToken);
                walker.assignments.RemoveAll(x => !IsMatch(currentSymbol, x.Right, semanticModel, cancellationToken));

                foreach (var declaration in walker.localDeclarations)
                {
                    if (declaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                        variableDeclaration.Variables.TryFirst(x => x.Initializer != null, out var variable) &&
                        IsMatch(currentSymbol, variable.Initializer.Value, semanticModel, cancellationToken) &&
                        semanticModel.TryGetSymbol(variable, cancellationToken, out ILocalSymbol? local))
                    {
                        using (var localWalker = With(local, currentNode))
                        {
                            walker.assignments.AddRange(localWalker.Assignments);
                        }
                    }
                }

                if (scope != SearchScope.Member)
                {
                    using (var currentVisited = visited.IncrementUsage())
                    {
                        foreach (var argument in walker.arguments)
                        {
                            if (argument.Expression is IdentifierNameSyntax identifierName &&
                                identifierName.Identifier.ValueText == currentSymbol.Name &&
                                argument.Parent is ArgumentListSyntax argumentList &&
                                semanticModel.TryGetSymbol(argumentList.Parent, cancellationToken, out IMethodSymbol? method) &&
                                method.TrySingleDeclaration(cancellationToken, out BaseMethodDeclarationSyntax? methodDeclaration) &&
                                method.TryFindParameter(argument, out var parameter) &&
                                currentVisited.Add(parameter))
                            {
                                using (var invocationWalker = With(parameter, methodDeclaration, currentVisited))
                                {
                                    walker.assignments.AddRange(invocationWalker.Assignments);
                                }
                            }
                        }

                        while (TryWalkBackingField(currentVisited, out var assignment, out var setterWalker))
                        {
                            walker.assignments.Remove(assignment);
                            walker.assignments.AddRange(setterWalker.assignments);
                        }
                    }
                }

                return walker;

                bool TryWalkBackingField(PooledSet<ISymbol> currentVisited, out AssignmentExpressionSyntax propertyAssignment, out AssignmentExecutionWalker setterWalker)
                {
                    propertyAssignment = null;
                    setterWalker = null;

                    foreach (var candidate in walker.assignments)
                    {
                        if (semanticModel.TryGetSymbol(candidate.Left, cancellationToken, out IPropertySymbol? property) &&
                            property.SetMethod is IMethodSymbol setMethod &&
                            setMethod.Parameters.TrySingle(out var parameter) &&
                            setMethod.TrySingleDeclaration(cancellationToken, out AccessorDeclarationSyntax? setter) &&
                            (setter.Body != null || setter.ExpressionBody != null) &&
                            currentVisited.Add(parameter))
                        {
                            propertyAssignment = candidate;
                            setterWalker = With(parameter, setter, currentVisited);
                            return true;
                        }
                    }

                    return false;
                }
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
        public static bool FirstFor(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out AssignmentExpressionSyntax? assignment)
        {
            assignment = null;
            if (symbol is null ||
                node is null)
            {
                return false;
            }

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
        public static bool SingleFor(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out AssignmentExpressionSyntax? assignment)
        {
            assignment = null;
            if (symbol is null ||
                node is null)
            {
                return false;
            }

            using (var walker = Borrow(node, scope, semanticModel, cancellationToken))
            {
                foreach (var candidate in walker.Assignments)
                {
                    if (semanticModel.TryGetSymbol(candidate.Left, cancellationToken, out ISymbol? assignedSymbol) &&
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
        public static bool FirstWith(ISymbol symbol, SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out AssignmentExpressionSyntax? assignment)
        {
            assignment = null;
            if (symbol is null ||
                node is null)
            {
                return false;
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
                case ConditionalExpressionSyntax conditional:
                    return IsMatch(symbol, conditional.WhenTrue, semanticModel, cancellationToken) ||
                           IsMatch(symbol, conditional.WhenFalse, semanticModel, cancellationToken);
                case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.CoalesceExpression):
                    return IsMatch(symbol, binary.Left, semanticModel, cancellationToken) ||
                           IsMatch(symbol, binary.Right, semanticModel, cancellationToken);
                case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AsExpression):
                    return IsMatch(symbol, binary.Left, semanticModel, cancellationToken);
                case CastExpressionSyntax cast:
                    return IsMatch(symbol, cast.Expression, semanticModel, cancellationToken);
                case ObjectCreationExpressionSyntax objectCreation when objectCreation.ArgumentList != null && objectCreation.ArgumentList.Arguments.TryFirst(x => SymbolComparer.Equals(symbol, semanticModel.GetSymbolSafe(x.Expression, cancellationToken)), out ArgumentSyntax _):
                    return true;
                default:
                    if (symbol.IsEither<ILocalSymbol, IParameterSymbol>())
                    {
                        return expression is IdentifierNameSyntax identifierName &&
                               identifierName.Identifier.ValueText == symbol.Name &&
                               SymbolComparer.Equals(symbol, semanticModel.GetSymbolSafe(expression, cancellationToken));
                    }

                    return SymbolComparer.Equals(symbol, semanticModel.GetSymbolSafe(expression, cancellationToken));
            }
        }
    }
}
