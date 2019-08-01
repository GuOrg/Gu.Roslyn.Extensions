namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeStyle;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable CA1724 // Type names should not match namespaces
    /// <summary>
    /// Helper for figuring out if the code uses underscore prefix in field names.
    /// </summary>
    public static class CodeStyle
#pragma warning restore CA1724 // Type names should not match namespaces
    {
        private enum Result
        {
            Unknown,
            Yes,
            No,
            Maybe,
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyFieldAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<bool> QualifyFieldAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var optionSet = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);
            if (optionSet.GetOption(CodeStyleOptions.QualifyFieldAccess, document.Project.Language) is CodeStyleOption<bool> option &&
                !ReferenceEquals(option, CodeStyleOptions.QualifyFieldAccess.DefaultValue))
            {
                return option.Value;
            }

            using (var walker = QualifyFieldAccessWalker.Borrow())
            {
                var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                switch (QualifiesFieldAccess(tree, walker))
                {
                    case Result.Unknown:
                        break;
                    case Result.Maybe:
                    case Result.Yes:
                        return true;
                    case Result.No:
                        return false;
                    default:
                        throw new InvalidOperationException("Not handling member.");
                }

                var compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    switch (QualifiesFieldAccess(syntaxTree, walker))
                    {
                        case Result.Unknown:
                            break;
                        case Result.Maybe:
                        case Result.Yes:
                            return true;
                        case Result.No:
                            return false;
                        default:
                            throw new InvalidOperationException("Not handling member.");
                    }
                }
            }

            return true;

            Result QualifiesFieldAccess(SyntaxTree tree, QualifyFieldAccessWalker walker)
            {
                if (IsExcluded(tree))
                {
                    return Result.Unknown;
                }

                if (tree.TryGetRoot(out var root))
                {
                    walker.Visit(root);
                    return walker.QualifiesAccess;
                }

                return Result.Unknown;
            }
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool UnderscoreFields(this SemanticModel semanticModel)
        {
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
                        throw new InvalidOperationException("Not handling member.");
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
                            throw new InvalidOperationException("Not handling member.");
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
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
                        throw new InvalidOperationException("Not handling member.");
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
                            throw new InvalidOperationException("Not handling member.");
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
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
                        throw new InvalidOperationException("Not handling member.");
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
                            throw new InvalidOperationException("Not handling member.");
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
                if (walker.UsesUnderScore == Result.No ||
                    walker.UsesUnderScore == Result.Maybe)
                {
                    return Result.No;
                }

                if (walker.UsesUnderScore == Result.Yes)
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
            return syntaxTree.FilePath.EndsWith(".g.i.cs", StringComparison.Ordinal) ||
                   syntaxTree.FilePath.EndsWith(".g.cs", StringComparison.Ordinal);
        }

        private sealed class QualifyFieldAccessWalker : PooledWalker<QualifyFieldAccessWalker>
        {
            private QualifyFieldAccessWalker()
            {
            }

            internal Result QualifiesAccess { get; private set; }

            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                switch (node.Parent)
                {
                    case AssignmentExpressionSyntax assignment when assignment.Left.Contains(node) &&
                                                                    !assignment.Parent.IsKind(SyntaxKind.ObjectInitializerExpression) &&
                                                                    IsMemberField():
                    case ArrowExpressionClauseSyntax _ when IsMemberField():
                    case ReturnStatementSyntax _ when IsMemberField():
                        switch (this.QualifiesAccess)
                        {
                            case Result.Unknown:
                                this.QualifiesAccess = Result.No;
                                break;
                            case Result.Yes:
                                this.QualifiesAccess = Result.Maybe;
                                break;
                            case Result.No:
                                break;
                            case Result.Maybe:
                                break;
                            default:
                                throw new InvalidOperationException($"Not handling {this.QualifiesAccess}");
                        }

                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                        memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                        IsMemberField():
                        switch (this.QualifiesAccess)
                        {
                            case Result.Unknown:
                                this.QualifiesAccess = Result.Yes;
                                break;
                            case Result.Yes:
                                break;
                            case Result.No:
                                this.QualifiesAccess = Result.Maybe;
                                break;
                            case Result.Maybe:
                                break;
                            default:
                                throw new InvalidOperationException($"Not handling {this.QualifiesAccess}");
                        }

                        break;
                }

                bool IsMemberField()
                {
                    return node.TryFirstAncestor(out MemberDeclarationSyntax containingMember) &&
                           !IsStatic(containingMember) &&
                           containingMember.Parent is TypeDeclarationSyntax containingType &&
                           containingType.TryFindField(node.Identifier.ValueText, out var field) &&
                           !field.Modifiers.Any(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword);

                    bool IsStatic(MemberDeclarationSyntax candidate)
                    {
                        switch (candidate)
                        {
                            case BaseMethodDeclarationSyntax declaration:
                                return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                            case BasePropertyDeclarationSyntax declaration:
                                return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                            default:
                                return true;
                        }
                    }
                }
            }

            internal static QualifyFieldAccessWalker Borrow() => Borrow(() => new QualifyFieldAccessWalker());

            protected override void Clear()
            {
                this.QualifiesAccess = Result.Unknown;
            }
        }

        private sealed class UnderscoreFieldWalker : PooledWalker<UnderscoreFieldWalker>
        {
            private UnderscoreFieldWalker()
            {
            }

            internal Result UsesUnderScore { get; private set; }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (node.Modifiers.Any(SyntaxKind.PrivateKeyword) && !node.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword))
                {
                    foreach (var variable in node.Declaration.Variables)
                    {
                        var name = variable.Identifier.ValueText;
                        if (name.StartsWith("_", StringComparison.Ordinal))
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
                                    throw new InvalidOperationException("Not handling member.");
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
                                    throw new InvalidOperationException("Not handling member.");
                            }
                        }
                    }
                }
            }

            internal static UnderscoreFieldWalker Borrow() => Borrow(() => new UnderscoreFieldWalker());

            protected override void Clear()
            {
                this.UsesUnderScore = Result.Unknown;
            }
        }

        private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
        {
            private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();

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

            internal static UsingDirectiveWalker Borrow() => Borrow(() => new UsingDirectiveWalker());

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

            internal static BackingFieldsAdjacentWalker Borrow() => Borrow(() => new BackingFieldsAdjacentWalker());

            internal Result Adjacent(out bool newLineBetween)
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
