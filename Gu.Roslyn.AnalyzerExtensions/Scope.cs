namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Exposes helper methods for the current scope.
    /// </summary>
    public static class Scope
    {
        /// <summary>
        /// Check if the current scope has a parameter named <paramref name="name"/>.
        /// </summary>
        /// <param name="nodeInScope">The node in the scope to check.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>True if the current scope has a parameter named <paramref name="name"/>.</returns>
        public static bool HasParameter(SyntaxNode nodeInScope, string name)
        {
            if (nodeInScope is null)
            {
                throw new System.ArgumentNullException(nameof(nodeInScope));
            }

            var scope = nodeInScope.Parent;
            while (scope != null)
            {
                switch (scope)
                {
                    case BaseMethodDeclarationSyntax method:
                        return HasParameter(method.ParameterList);
                    case AccessorDeclarationSyntax accessor:
                        switch (accessor.Kind())
                        {
                            case SyntaxKind.AddAccessorDeclaration:
                            case SyntaxKind.RemoveAccessorDeclaration:
                            case SyntaxKind.SetAccessorDeclaration:
                                return name == "value";
                        }

                        return false;
                    case ParenthesizedLambdaExpressionSyntax lambda when HasParameter(lambda.ParameterList):
                    case SimpleLambdaExpressionSyntax simpleLambda when simpleLambda.Parameter?.Identifier.Text == name:
                    case LocalFunctionStatementSyntax localFunction when HasParameter(localFunction.ParameterList):
                        return true;
                }

                scope = scope.Parent;
            }

            return false;

            bool HasParameter(ParameterListSyntax parameterList)
            {
                return parameterList.TryFind(name, out _);
            }
        }

        /// <summary>
        /// Check if the current scope has a local named <paramref name="name"/>.
        /// </summary>
        /// <param name="nodeInScope">The node in the scope to check.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>True if the current scope has a local named <paramref name="name"/>.</returns>
        public static bool HasLocal(SyntaxNode nodeInScope, string name)
        {
            if (nodeInScope is null)
            {
                throw new System.ArgumentNullException(nameof(nodeInScope));
            }

            var scope = nodeInScope.Parent;
            while (scope != null)
            {
                switch (scope)
                {
                    case BaseMethodDeclarationSyntax _:
                    case AccessorDeclarationSyntax _:
                        return false;
                    case BlockSyntax block when DeclaresLocal(block):
                        return true;
                    case SwitchSectionSyntax switchSection:
                        foreach (var label in switchSection.Labels)
                        {
                            if (label is CasePatternSwitchLabelSyntax casePattern &&
                                casePattern.Pattern is DeclarationPatternSyntax declarationPattern &&
                                DesignatesLocal(declarationPattern.Designation))
                            {
                                return true;
                            }
                        }

                        break;
                }

                scope = scope.Parent;
            }

            return false;

            bool DeclaresLocal(BlockSyntax block)
            {
                foreach (var statement in block.Statements)
                {
                    switch (statement)
                    {
                        case LocalDeclarationStatementSyntax localDeclaration when localDeclaration.Declaration is VariableDeclarationSyntax declaration:
                            foreach (var variable in declaration.Variables)
                            {
                                if (IsMatch(variable.Identifier))
                                {
                                    return true;
                                }
                            }

                            break;
                        case IfStatementSyntax ifStatement when ifStatement.Condition is ExpressionSyntax condition:
                            foreach (SyntaxNode node in condition.DescendantNodes())
                            {
                                switch (node)
                                {
                                    case DeclarationExpressionSyntax declaration when declaration.Designation is SingleVariableDesignationSyntax variable &&
                                                                                      IsMatch(variable.Identifier):
                                        return true;
                                    case DeclarationPatternSyntax declaration when DesignatesLocal(declaration.Designation):
                                        return true;
                                }
                            }

                            break;
                    }
                }

                return false;
            }

            bool DesignatesLocal(VariableDesignationSyntax candidate)
            {
                switch (candidate)
                {
                    case SingleVariableDesignationSyntax variable:
                        return IsMatch(variable.Identifier);
                    default:
                        return true;
                }
            }

            bool IsMatch(SyntaxToken identifier) => identifier.Text == name || identifier.ValueText == name;
        }

        /// <summary>
        /// Check if the node is in static context where this is not accessible.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns>True if the node is in static context where this is not accessible.</returns>
        public static bool IsInStaticContext(this SyntaxNode node)
        {
            if (node.TryFirstAncestor(out MemberDeclarationSyntax memberDeclaration))
            {
                switch (memberDeclaration)
                {
                    case FieldDeclarationSyntax declaration:
                        return declaration.Modifiers.Any(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword) ||
                               (declaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                                variableDeclaration.Variables.TryLast(out var last) &&
                                last.Initializer.Contains(node));
                    case BaseFieldDeclarationSyntax declaration:
                        return declaration.Modifiers.Any(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword);
                    case PropertyDeclarationSyntax declaration:
                        return declaration.Modifiers.Any(SyntaxKind.StaticKeyword) ||
                               declaration.Initializer?.Contains(node) == true;
                    case BasePropertyDeclarationSyntax declaration:
                        return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                    case BaseMethodDeclarationSyntax declaration:
                        return declaration.Modifiers.Any(SyntaxKind.StaticKeyword) ||
                               node.TryFirstAncestor<ConstructorInitializerSyntax>(out _);
                    default:
                        return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if <paramref name="statement"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="statement">The <see cref="StatementSyntax"/>.</param>
        /// <param name="other">The other <see cref="StatementSyntax"/>.</param>
        /// <returns>Null if the execution order could not be figured out.</returns>
        public static ExecutedBefore IsExecutedBefore(this StatementSyntax statement, StatementSyntax other)
        {
            if (statement == null ||
                other == null)
            {
                return ExecutedBefore.Unknown;
            }

            if (SyntaxExecutionContext.IsInLambda(statement, other, out var executedBefore))
            {
                return executedBefore;
            }

            if ((statement.Parent as BlockSyntax)?.ContainsGoto() == true)
            {
                return ExecutedBefore.Maybe;
            }

            if (statement.SpanStart >= other.SpanStart &&
                (statement.TryFindSharedAncestorRecursive(other, out DoStatementSyntax _) ||
                 statement.TryFindSharedAncestorRecursive(other, out ForStatementSyntax _) ||
                 statement.TryFindSharedAncestorRecursive(other, out ForEachStatementSyntax _) ||
                 statement.TryFindSharedAncestorRecursive(other, out WhileStatementSyntax _)))
            {
                return ExecutedBefore.Maybe;
            }

            if (ReferenceEquals(statement, other))
            {
                return ExecutedBefore.No;
            }

            if (ReferenceEquals(statement.Parent, other.Parent))
            {
                return statement.SpanStart < other.SpanStart ? ExecutedBefore.Yes : ExecutedBefore.No;
            }

            if (!statement.SharesAncestor<MemberDeclarationSyntax>(other, out _))
            {
                return ExecutedBefore.Unknown;
            }

            if (statement.IsInParentBlock(other))
            {
                if (statement.SpanStart < other.SpanStart)
                {
                    return ExecutedBefore.Yes;
                }

                return ExecutedBefore.No;
            }

            if (other.IsInParentBlock(statement))
            {
                if (statement.SpanStart > other.SpanStart)
                {
                    return ExecutedBefore.No;
                }

                if (statement.Parent is BlockSyntax block &&
                    (block.Statements.TryFirstOfType(out ReturnStatementSyntax _) ||
                     block.Statements.TryFirstOfType(out ThrowStatementSyntax _)))
                {
                    return ExecutedBefore.No;
                }

                if (statement.TryFirstAncestor(out CatchClauseSyntax _))
                {
                    return statement.SpanStart < other.SpanStart ? ExecutedBefore.Maybe : ExecutedBefore.No;
                }

                if (statement.SpanStart < other.SpanStart)
                {
                    if (statement.TryFirstAncestor<SwitchStatementSyntax>(out _) ||
                        statement.TryFirstAncestor<IfStatementSyntax>(out _))
                    {
                        return ExecutedBefore.Maybe;
                    }

                    return ExecutedBefore.Yes;
                }

                return ExecutedBefore.No;
            }

            if (statement.TryFindSharedAncestorRecursive(other, out IfStatementSyntax ifStatement) &&
                ((ifStatement.Statement?.Contains(statement) == true && ifStatement.Else?.Statement?.Contains(other) == true) ||
                 (ifStatement.Statement?.Contains(other) == true && ifStatement.Else?.Statement?.Contains(statement) == true)))
            {
                return ExecutedBefore.No;
            }

            if (statement.TryFirstAncestor(out TryStatementSyntax tryStatement))
            {
                if (tryStatement.Block.Contains(statement))
                {
                    if (other.TryFirstAncestor(out CatchClauseSyntax catchClause) &&
                        tryStatement.Catches.TryFirst(x => x == catchClause, out _))
                    {
                        return ExecutedBefore.Yes;
                    }

                    if (tryStatement.Finally?.Contains(other) == true)
                    {
                        return ExecutedBefore.Yes;
                    }
                }
                else if (statement.TryFirstAncestor(out CatchClauseSyntax _))
                {
                    if (other.TryFirstAncestor(out CatchClauseSyntax _))
                    {
                        return ExecutedBefore.No;
                    }

                    return statement.SpanStart < other.SpanStart ? ExecutedBefore.Maybe : ExecutedBefore.No;
                }
                else if (other.TryFirstAncestor(out CatchClauseSyntax _))
                {
                    return statement.SpanStart < other.SpanStart ? ExecutedBefore.Maybe : ExecutedBefore.No;
                }

                return statement.SpanStart < other.SpanStart ? ExecutedBefore.Yes : ExecutedBefore.No;
            }

            if (statement.SharesAncestor<SwitchStatementSyntax>(other, out _))
            {
                return ExecutedBefore.No;
            }

            return ExecutedBefore.Unknown;
        }

        /// <summary>
        /// Check if <paramref name="statement"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="statement">The <see cref="StatementSyntax"/>.</param>
        /// <param name="other">The <see cref="ExpressionSyntax"/>.</param>
        /// <returns>Null if the execution order could not be figured out.</returns>
        public static ExecutedBefore IsExecutedBefore(this StatementSyntax statement, ExpressionSyntax other)
        {
            if (statement == null)
            {
                throw new System.ArgumentNullException(nameof(statement));
            }

            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            if (SyntaxExecutionContext.IsInLambda(statement, other, out var executedBefore))
            {
                return executedBefore;
            }

            if (other.TryFirstAncestor(out StatementSyntax otherStatement))
            {
                return statement.IsExecutedBefore(otherStatement);
            }

            return ExecutedBefore.Unknown;
        }

        /// <summary>
        /// Tries to determine if <paramref name="node"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <returns>Null if it could not be determined.</returns>
        public static ExecutedBefore IsExecutedBefore(this ExpressionSyntax node, ExpressionSyntax other)
        {
            if (node is null ||
                other is null)
            {
                return ExecutedBefore.Unknown;
            }

            if (ReferenceEquals(node, other))
            {
                return ExecutedBefore.No;
            }

            if (node.Contains(other))
            {
                return ExecutedBefore.No;
            }

            if (other.Contains(node))
            {
                return ExecutedBefore.Yes;
            }

            if (SyntaxExecutionContext.IsInLambda(node, other, out var executedBefore))
            {
                return executedBefore;
            }

            if (node.TryFirstAncestor(out StatementSyntax statement) &&
                other.TryFirstAncestor(out StatementSyntax otherStatement))
            {
                return statement.IsExecutedBefore(otherStatement);
            }

            if (node.TryFindSharedAncestorRecursive(other, out BinaryExpressionSyntax _))
            {
                return node.SpanStart < other.SpanStart ? ExecutedBefore.Yes : ExecutedBefore.No;
            }

            return ExecutedBefore.Unknown;
        }

        /// <summary>
        /// Tries to determine if <paramref name="node"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <returns>Null if it could not be determined.</returns>
        public static ExecutedBefore IsExecutedBefore(this ExpressionSyntax node, StatementSyntax other)
        {
            if (node is null ||
                other is null)
            {
                return ExecutedBefore.Unknown;
            }

            if (SyntaxExecutionContext.IsInLambda(node, other, out var executedBefore))
            {
                return executedBefore;
            }

            if (node.TryFirstAncestor(out StatementSyntax statement))
            {
                return statement.IsExecutedBefore(other);
            }

            return ExecutedBefore.Unknown;
        }

        /// <summary>
        /// Check if <paramref name="statement"/> or statement.Parent contains the block <paramref name="other"/> is in.
        /// </summary>
        /// <param name="statement">The <see cref="StatementSyntax"/>.</param>
        /// <param name="other">The other <see cref="StatementSyntax"/>.</param>
        /// <returns>True if <paramref name="statement"/> or statement.Parent contains the block <paramref name="other"/> is in.</returns>
        private static bool IsInParentBlock(this StatementSyntax statement, StatementSyntax other)
        {
            if (statement == null || other == null)
            {
                return false;
            }

            if (ReferenceEquals(statement, other) ||
                ReferenceEquals(statement.Parent, other.Parent))
            {
                return false;
            }

            return statement.Parent is BlockSyntax block &&
                   block.Contains(other);
        }
    }
}
