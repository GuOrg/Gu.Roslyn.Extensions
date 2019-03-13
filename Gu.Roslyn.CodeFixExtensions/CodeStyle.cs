namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for figuring out if the code uses underscore prefix in field names.
    /// </summary>
    public static class CodeStyle
    {
        private enum Result
        {
            Unknown,
            Yes,
            No,
            Maybe,
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool UnderscoreFields(this SemanticModel semanticModel)
        {
            using (var walker = UnderscoreFieldWalker.Borrow())
            {
                switch (UnderscoreFields(semanticModel.SyntaxTree, walker))
                {
                    case Result.Unknown:
                    case Result.Maybe:
                        break;
                    case Result.Yes:
                        return true;
                    case Result.No:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var tree in semanticModel.Compilation.SyntaxTrees)
                {
                    switch (UnderscoreFields(tree, walker))
                    {
                        case Result.Unknown:
                        case Result.Maybe:
                            break;
                        case Result.Yes:
                            return true;
                        case Result.No:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool UsingDirectivesInsideNamespace(SemanticModel semanticModel)
        {
            using (var walker = UsingDirectiveWalker.Borrow())
            {
                switch (UsingDirectivesInsideNamespace(semanticModel.SyntaxTree, walker))
                {
                    case Result.Unknown:
                    case Result.Maybe:
                        break;
                    case Result.Yes:
                        return true;
                    case Result.No:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var tree in semanticModel.Compilation.SyntaxTrees)
                {
                    switch (UsingDirectivesInsideNamespace(tree, walker))
                    {
                        case Result.Unknown:
                        case Result.Maybe:
                            break;
                        case Result.Yes:
                            return true;
                        case Result.No:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool BackingFieldsAdjacent(this SemanticModel semanticModel, out bool newLineBetween)
        {
            using (var walker = BackingFieldsAdjacentWalker.Borrow())
            {
                switch (BackingFieldsAdjacent(semanticModel.SyntaxTree, walker, out newLineBetween))
                {
                    case Result.Unknown:
                    case Result.Maybe:
                        break;
                    case Result.Yes:
                        return true;
                    case Result.No:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var tree in semanticModel.Compilation.SyntaxTrees)
                {
                    switch (BackingFieldsAdjacent(tree, walker, out newLineBetween))
                    {
                        case Result.Unknown:
                        case Result.Maybe:
                            break;
                        case Result.Yes:
                            return true;
                        case Result.No:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            newLineBetween = false;
            return false;
        }

        private static Result UnderscoreFields(this SyntaxTree tree, UnderscoreFieldWalker walker)
        {
            if (IsExcluded(tree))
            {
                return Result.Unknown;
            }

            if (tree.TryGetRoot(out var root))
            {
                walker.Visit(root);
                if (walker.UsesThis == Result.Yes ||
                    walker.UsesUnderScore == Result.No)
                {
                    return Result.No;
                }

                if (walker.UsesUnderScore == Result.Yes ||
                    walker.UsesThis == Result.No)
                {
                    return Result.Yes;
                }
            }

            return Result.Unknown;
        }

        private static Result UsingDirectivesInsideNamespace(this SyntaxTree tree, UsingDirectiveWalker walker)
        {
            if (IsExcluded(tree))
            {
                return Result.Unknown;
            }

            if (tree.TryGetRoot(out var root))
            {
                walker.Visit(root);
            }

            return walker.UsingDirectivesInside();
        }

        private static Result BackingFieldsAdjacent(this SyntaxTree tree, BackingFieldsAdjacentWalker walker, out bool newLineBetween)
        {
            if (IsExcluded(tree))
            {
                newLineBetween = false;
                return Result.Unknown;
            }

            if (tree.TryGetRoot(out var root))
            {
                walker.Visit(root);
            }

            return walker.Adjacent(out newLineBetween);
        }

        private static bool IsExcluded(SyntaxTree syntaxTree)
        {
            return syntaxTree.FilePath.EndsWith(".g.i.cs") ||
                   syntaxTree.FilePath.EndsWith(".g.cs");
        }

        private sealed class UnderscoreFieldWalker : PooledWalker<UnderscoreFieldWalker>
        {
            private UnderscoreFieldWalker()
            {
            }

            public Result UsesThis { get; private set; }

            public Result UsesUnderScore { get; private set; }

            public static UnderscoreFieldWalker Borrow() => Borrow(() => new UnderscoreFieldWalker());

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (node.IsMissing ||
                    node.Modifiers.Any(SyntaxKind.StaticKeyword) ||
                    node.Modifiers.Any(SyntaxKind.ConstKeyword) ||
                    node.Modifiers.Any(SyntaxKind.PublicKeyword) ||
                    node.Modifiers.Any(SyntaxKind.ProtectedKeyword) ||
                    node.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return;
                }

                foreach (var variable in node.Declaration.Variables)
                {
                    var name = variable.Identifier.ValueText;
                    if (name.StartsWith("_"))
                    {
                        switch (this.UsesUnderScore)
                        {
                            case Result.Unknown:
                                this.UsesUnderScore = Result.Yes;
                                break;
                            case Result.Yes:
                                break;
                            case Result.No:
                                this.UsesUnderScore = Result.Maybe;
                                break;
                            case Result.Maybe:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        switch (this.UsesUnderScore)
                        {
                            case Result.Unknown:
                                this.UsesUnderScore = Result.No;
                                break;
                            case Result.Yes:
                                this.UsesUnderScore = Result.Maybe;
                                break;
                            case Result.No:
                                break;
                            case Result.Maybe:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            public override void VisitThisExpression(ThisExpressionSyntax node)
            {
                switch (node.Parent.Kind())
                {
                    case SyntaxKind.Argument:
                        return;
                }

                switch (this.UsesThis)
                {
                    case Result.Unknown:
                        this.UsesThis = Result.Yes;
                        break;
                    case Result.Yes:
                        break;
                    case Result.No:
                        this.UsesThis = Result.Maybe;
                        break;
                    case Result.Maybe:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                if (node.Parent is InitializerExpressionSyntax)
                {
                    return;
                }

                this.CheckUsesThis(node.Left);
                base.VisitAssignmentExpression(node);
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                this.CheckUsesThis(node.Expression);
                base.VisitInvocationExpression(node);
            }

            public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                this.CheckUsesThis(node);
                base.VisitMemberAccessExpression(node);
            }

            public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
            {
                this.CheckUsesThis(node.Expression);
                base.VisitConditionalAccessExpression(node);
            }

            protected override void Clear()
            {
                this.UsesThis = Result.Unknown;
                this.UsesUnderScore = Result.Unknown;
            }

            private void CheckUsesThis(ExpressionSyntax expression)
            {
                if (expression == null ||
                    this.UsesThis != Result.Unknown)
                {
                    return;
                }

                if (expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Expression is ThisExpressionSyntax)
                {
                    switch (this.UsesThis)
                    {
                        case Result.Unknown:
                            this.UsesThis = Result.Yes;
                            break;
                        case Result.Yes:
                            break;
                        case Result.No:
                            this.UsesThis = Result.Maybe;
                            break;
                        case Result.Maybe:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (expression is IdentifierNameSyntax identifierName &&
                    expression.FirstAncestor<TypeDeclarationSyntax>() is TypeDeclarationSyntax typeDeclaration)
                {
                    if (typeDeclaration.TryFindField(identifierName.Identifier.ValueText, out var field) &&
                        (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword)))
                    {
                        return;
                    }

                    if (typeDeclaration.TryFindProperty(identifierName.Identifier.ValueText, out var property) &&
                        property.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    if (typeDeclaration.TryFindMethod(identifierName.Identifier.ValueText, out var method) &&
                        method.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    switch (this.UsesThis)
                    {
                        case Result.Unknown:
                            this.UsesThis = Result.No;
                            break;
                        case Result.Yes:
                            this.UsesThis = Result.Maybe;
                            break;
                        case Result.No:
                            break;
                        case Result.Maybe:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
        {
            private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();

            public static UsingDirectiveWalker Borrow() => Borrow(() => new UsingDirectiveWalker());

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                this.usingDirectives.Add(node);
                base.VisitUsingDirective(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // Stop walking here
            }

            internal Result UsingDirectivesInside()
            {
                if (this.usingDirectives.Count == 0)
                {
                    return Result.Unknown;
                }

                if (this.usingDirectives.TryFirst(x => x.FirstAncestor<NamespaceDeclarationSyntax>() != null, out _))
                {
                    return this.usingDirectives.TryFirst(x => x.FirstAncestor<NamespaceDeclarationSyntax>() == null, out _)
                        ? Result.Maybe
                        : Result.Yes;
                }

                return Result.No;
            }

            protected override void Clear()
            {
                this.usingDirectives.Clear();
            }
        }

        private sealed class BackingFieldsAdjacentWalker : PooledWalker<BackingFieldsAdjacentWalker>
        {
            private Result result;
            private bool newLine;

            public static BackingFieldsAdjacentWalker Borrow() => Borrow(() => new BackingFieldsAdjacentWalker());

            public override void VisitReturnStatement(ReturnStatementSyntax node)
            {
                if (node.Parent is BlockSyntax block &&
                    block.Parent is AccessorDeclarationSyntax accessor &&
                    accessor.Parent is AccessorListSyntax accessorList &&
                    accessorList.Parent is PropertyDeclarationSyntax property)
                {
                    this.TryUpdateAdjacent(property, node.Expression);
                }

                base.VisitReturnStatement(node);
            }

            public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
            {
                switch (node.Parent)
                {
                    case AccessorDeclarationSyntax accessor when accessor.Parent is AccessorListSyntax accessorList &&
                                                                 accessorList.Parent is PropertyDeclarationSyntax property:
                        this.TryUpdateAdjacent(property, node.Expression);
                        break;
                    case PropertyDeclarationSyntax property:
                        this.TryUpdateAdjacent(property, node.Expression);
                        break;
                }

                base.VisitArrowExpressionClause(node);
            }

            public Result Adjacent(out bool newLineBetween)
            {
                newLineBetween = this.newLine;
                return this.result;
            }

            protected override void Clear()
            {
                this.result = Result.Unknown;
            }

            private void TryUpdateAdjacent(PropertyDeclarationSyntax property, ExpressionSyntax returnValue)
            {
                switch (returnValue)
                {
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                        this.TryUpdateAdjacent(property, memberAccess.Name.Identifier.ValueText);
                        break;
                    case IdentifierNameSyntax identifierName:
                        this.TryUpdateAdjacent(property, identifierName.Identifier.ValueText);
                        break;
                }
            }

            private void TryUpdateAdjacent(PropertyDeclarationSyntax property, string candidate)
            {
                if (property.Parent is TypeDeclarationSyntax typeDeclaration &&
                    typeDeclaration.TryFindField(candidate, out var field))
                {
                    var index = typeDeclaration.Members.IndexOf(property);
                    if (index > 0 &&
                        typeDeclaration.Members[index - 1] == field)
                    {
                        for (var i = index - 2; i >= 0; i--)
                        {
                            if (!(typeDeclaration.Members[index] is FieldDeclarationSyntax))
                            {
                                this.result = Result.Yes;
                                this.newLine = property.HasLeadingTrivia &&
                                               property.GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia);
                            }
                        }

                        if (!property.HasLeadingTrivia ||
                            !property.GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia))
                        {
                            this.result = Result.Yes;
                            this.newLine = false;
                        }
                    }
                }
            }
        }
    }
}
