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
    using Microsoft.CodeAnalysis.Editing;

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
            Mixed,
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
        public static async Task<bool?> QualifyFieldAccessAsync(this Document document, CancellationToken cancellationToken)
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

            var result = await QualifyFieldAccessWalker.CheckAsync(document, cancellationToken)
                                                       .ConfigureAwait(false);
            switch (result)
            {
                case Result.Unknown:
                    return null;
                case Result.Mixed:
                case Result.Yes:
                    return true;
                case Result.No:
                    return false;
                default:
                    throw new InvalidOperationException($"Not handling {result}");
            }
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyPropertyAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<bool?> QualifyPropertyAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var optionSet = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);
            if (optionSet.GetOption(CodeStyleOptions.QualifyPropertyAccess, document.Project.Language) is CodeStyleOption<bool> option &&
                !ReferenceEquals(option, CodeStyleOptions.QualifyPropertyAccess.DefaultValue))
            {
                return option.Value;
            }

            var result = await QualifyPropertyAccessWalker.CheckAsync(document, cancellationToken)
                                                       .ConfigureAwait(false);
            switch (result)
            {
                case Result.Unknown:
                    return null;
                case Result.Mixed:
                case Result.Yes:
                    return true;
                case Result.No:
                    return false;
                default:
                    throw new InvalidOperationException($"Not handling {result}");
            }
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyMethodAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<bool?> QualifyMethodAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var optionSet = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);
            if (optionSet.GetOption(CodeStyleOptions.QualifyMethodAccess, document.Project.Language) is CodeStyleOption<bool> option &&
                !ReferenceEquals(option, CodeStyleOptions.QualifyMethodAccess.DefaultValue))
            {
                return option.Value;
            }

            var result = await QualifyMethodAccessWalker.CheckAsync(document, cancellationToken)
                                                          .ConfigureAwait(false);
            switch (result)
            {
                case Result.Unknown:
                    return null;
                case Result.Mixed:
                case Result.Yes:
                    return true;
                case Result.No:
                    return false;
                default:
                    throw new InvalidOperationException($"Not handling {result}");
            }
        }

        /// <summary>
        /// Figuring out if field names should be prefixed with _.
        /// 1. Walk current <paramref name="document"/>.
        /// 2. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<bool?> UnderscoreFieldsAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var result = await UnderscoreFieldWalker.CheckAsync(document, cancellationToken)
                                                    .ConfigureAwait(false);
            switch (result)
            {
                case Result.Unknown:
                    return null;
                case Result.Mixed:
                case Result.Yes:
                    return true;
                case Result.No:
                    return false;
                default:
                    throw new InvalidOperationException($"Not handling {result}");
            }
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool? UnderscoreFields(DocumentEditor editor) => UnderscoreFields(editor?.SemanticModel);

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool? UnderscoreFields(this SemanticModel semanticModel)
        {
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            var result = UnderscoreFieldWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation);
            switch (result)
            {
                case Result.Unknown:
                    return null;
                case Result.Mixed:
                case Result.Yes:
                    return true;
                case Result.No:
                    return false;
                default:
                    throw new InvalidOperationException($"Not handling {result}");
            }
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool? UsingDirectivesInsideNamespace(SemanticModel semanticModel)
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
                        break;
                    case Result.Mixed:
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
                            break;
                        case Result.Mixed:
                        case Result.Yes:
                            return true;
                        case Result.No:
                            return false;
                        default:
                            throw new InvalidOperationException("Not handling member.");
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool? UsingDirectivesInsideNamespace(DocumentEditor editor) => UsingDirectivesInsideNamespace(editor?.SemanticModel);

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static bool? BackingFieldsAdjacent(this SemanticModel semanticModel, out bool newLineBetween)
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
                    case Result.Mixed:
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
                        case Result.Mixed:
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
            return null;
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

        private sealed class QualifyFieldAccessWalker : TreeWalker<QualifyFieldAccessWalker>
        {
            private QualifyFieldAccessWalker()
            {
            }

            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                switch (node.Parent)
                {
                    case AssignmentExpressionSyntax assignment when assignment.Left.Contains(node) &&
                                                                    !assignment.Parent.IsKind(SyntaxKind.ObjectInitializerExpression) &&
                                                                    IsMemberField():
                    case ArrowExpressionClauseSyntax _ when IsMemberField():
                    case ReturnStatementSyntax _ when IsMemberField():
                        this.Update(Result.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                        memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                        IsMemberField():
                        this.Update(Result.Yes);
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

            internal static async Task<Result> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyFieldAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class QualifyPropertyAccessWalker : TreeWalker<QualifyPropertyAccessWalker>
        {
            private QualifyPropertyAccessWalker()
            {
            }

            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                switch (node.Parent)
                {
                    case AssignmentExpressionSyntax assignment when assignment.Left.Contains(node) &&
                                                                    !assignment.Parent.IsKind(SyntaxKind.ObjectInitializerExpression) &&
                                                                    IsMemberProperty():
                    case ArrowExpressionClauseSyntax _ when IsMemberProperty():
                    case ReturnStatementSyntax _ when IsMemberProperty():
                        this.Update(Result.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                        memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                        IsMemberProperty():
                        this.Update(Result.Yes);
                        break;
                }

                bool IsMemberProperty()
                {
                    return node.TryFirstAncestor(out MemberDeclarationSyntax containingMember) &&
                           !IsStatic(containingMember) &&
                           containingMember.Parent is TypeDeclarationSyntax containingType &&
                           containingType.TryFindProperty(node.Identifier.ValueText, out var property) &&
                           !property.Modifiers.Any(SyntaxKind.StaticKeyword);

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

            internal static async Task<Result> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyPropertyAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class QualifyMethodAccessWalker : TreeWalker<QualifyMethodAccessWalker>
        {
            private QualifyMethodAccessWalker()
            {
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                switch (node.Expression)
                {
                    case IdentifierNameSyntax _ when IsMemberMethod():
                        this.Update(Result.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression.IsKind(SyntaxKind.ThisExpression):
                        this.Update(Result.Yes);
                        break;
                }

                bool IsMemberMethod()
                {
                    return node.TryFirstAncestor(out MemberDeclarationSyntax containingMember) &&
                           !IsStatic(containingMember) &&
                           containingMember.Parent is TypeDeclarationSyntax containingType &&
                           node.TryGetMethodName(out var name) &&
                           containingType.TryFindMethod(name, out var method) &&
                           !method.Modifiers.Any(SyntaxKind.StaticKeyword);

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

                base.VisitInvocationExpression(node);
            }

            internal static async Task<Result> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyMethodAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class UnderscoreFieldWalker : TreeWalker<UnderscoreFieldWalker>
        {
            private UnderscoreFieldWalker()
            {
            }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (node.Modifiers.Any(SyntaxKind.PrivateKeyword) && !node.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword))
                {
                    foreach (var variable in node.Declaration.Variables)
                    {
                        var name = variable.Identifier.ValueText;
                        this.Update(name.StartsWith("_", StringComparison.Ordinal) ? Result.Yes : Result.No);
                    }
                }
            }

            internal static async Task<Result> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new UnderscoreFieldWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }

            internal static Result Check(SyntaxTree containing, Compilation compilation)
            {
                using (var walker = Borrow(() => new UnderscoreFieldWalker()))
                {
                    return walker.CheckCore(containing, compilation);
                }
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
                        ? Result.Mixed
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

        private abstract class TreeWalker<T> : PooledWalker<T>
            where T : TreeWalker<T>
        {
            private Result result = Result.Unknown;

            protected async Task<Result> CheckCoreAsync(Document containing, CancellationToken cancellationToken)
            {
                if (await Check(containing).ConfigureAwait(false) is Result containingResult &&
                    containingResult != Result.Unknown)
                {
                    return containingResult;
                }

                foreach (var document in containing.Project.Documents)
                {
                    if (document == containing)
                    {
                        continue;
                    }

                    if (await Check(document).ConfigureAwait(false) is Result documentResult &&
                        documentResult != Result.Unknown)
                    {
                        return documentResult;
                    }
                }

                return Result.Unknown;

                async Task<Result> Check(Document candidate)
                {
                    var tree = await candidate.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                    if (IsExcluded(tree))
                    {
                        return Result.Unknown;
                    }

                    if (tree.TryGetRoot(out var root))
                    {
                        this.Visit(root);
                        return this.result;
                    }

                    return Result.Unknown;
                }
            }

            protected Result CheckCore(SyntaxTree containing, Compilation compilation)
            {
                if (Check(containing) is Result containingResult &&
                    containingResult != Result.Unknown)
                {
                    return containingResult;
                }

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (syntaxTree == containing)
                    {
                        continue;
                    }

                    if (Check(syntaxTree) is Result syntaxTreeResult &&
                        syntaxTreeResult != Result.Unknown)
                    {
                        return syntaxTreeResult;
                    }
                }

                return Result.Unknown;

                Result Check(SyntaxTree tree)
                {
                    if (IsExcluded(tree))
                    {
                        return Result.Unknown;
                    }

                    if (tree.TryGetRoot(out var root))
                    {
                        this.Visit(root);
                        return this.result;
                    }

                    return Result.Unknown;
                }
            }

            protected void Update(Result newValue)
            {
                switch (this.result)
                {
                    case Result.Unknown:
                        this.result = newValue;
                        break;
                    case Result.Yes:
                        if (newValue == Result.No)
                        {
                            this.result = Result.Mixed;
                        }

                        break;
                    case Result.No:
                        if (newValue == Result.Yes)
                        {
                            this.result = Result.Mixed;
                        }

                        break;
                    case Result.Mixed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
                }
            }

            protected override void Clear()
            {
                this.result = Result.Unknown;
            }
        }
    }
}
