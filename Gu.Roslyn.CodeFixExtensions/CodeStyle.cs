namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeStyle;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Options;

#pragma warning disable CA1724 // Type names should not match namespaces
    /// <summary>
    /// Helper for figuring out if the code uses underscore prefix in field names.
    /// </summary>
    public static class CodeStyle
#pragma warning restore CA1724 // Type names should not match namespaces
    {
        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyFieldAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> QualifyFieldAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyFieldAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyFieldAccessWalker.CheckAsync(document, cancellationToken)
                                                 .ConfigureAwait(false);
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
        public static async Task<CodeStyleResult> QualifyPropertyAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyPropertyAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyPropertyAccessWalker.CheckAsync(document, cancellationToken)
                                                    .ConfigureAwait(false);
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
        public static async Task<CodeStyleResult> QualifyMethodAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyMethodAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyMethodAccessWalker.CheckAsync(document, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if field names should be prefixed with _.
        /// 1. Walk current <paramref name="document"/>.
        /// 2. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> UnderscoreFieldsAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return await UnderscoreFieldWalker.CheckAsync(document, cancellationToken)
                                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UnderscoreFields(DocumentEditor editor) => UnderscoreFields(editor?.SemanticModel);

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UnderscoreFields(this SemanticModel semanticModel)
        {
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return UnderscoreFieldWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation);
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UsingDirectivesInsideNamespace(SemanticModel semanticModel)
        {
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return UsingDirectivesInsideNamespaceWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation);
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UsingDirectivesInsideNamespace(DocumentEditor editor) => UsingDirectivesInsideNamespace(editor?.SemanticModel);

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult BackingFieldsAdjacent(this SemanticModel semanticModel, out bool newLineBetween)
        {
            if (semanticModel == null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return BackingFieldsAdjacentWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation, out newLineBetween);
        }

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult BackingFieldsAdjacent(this DocumentEditor editor, out bool newLineBetween) => BackingFieldsAdjacent(editor?.SemanticModel, out newLineBetween);

        /// <summary>
        /// Find the <see cref="CodeStyleOption{T}"/> for <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="key">The <see cref="PerLanguageOption{T}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns><see cref="CodeStyleOption{T}"/> or null if not found.</returns>
        public static async Task<CodeStyleOption<T>> FindInEditorConfigAsync<T>(Document document, PerLanguageOption<CodeStyleOption<T>> key, CancellationToken cancellationToken)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var optionSet = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);
            if (optionSet.GetOption(key, document.Project.Language) is CodeStyleOption<T> option &&
                !ReferenceEquals(option, key.DefaultValue))
            {
                return option;
            }

            return null;
        }

        private static bool IsExcluded(SyntaxTree syntaxTree)
        {
            return syntaxTree.FilePath.EndsWith(".g.i.cs", StringComparison.Ordinal) ||
                   syntaxTree.FilePath.EndsWith(".g.cs", StringComparison.Ordinal);
        }

        /// <summary>
        /// Base class for walking trees in a compilation.
        /// </summary>
        /// <typeparam name="T">The walker type.</typeparam>
        public abstract class CompilationWalker<T> : PooledWalker<T>
            where T : CompilationWalker<T>
        {
            private CodeStyleResult result = CodeStyleResult.NotFound;

            /// <summary>
            /// Walk <paramref name="containing"/> then all documents in project.
            /// </summary>
            /// <param name="containing">The <see cref="Document"/>.</param>
            /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
            /// <returns>The <see cref="CodeStyleResult"/> found.</returns>
            protected async Task<CodeStyleResult> CheckCoreAsync(Document containing, CancellationToken cancellationToken)
            {
                if (await Check(containing).ConfigureAwait(false) is CodeStyleResult containingResult &&
                    containingResult != CodeStyleResult.NotFound)
                {
                    return containingResult;
                }

                foreach (var document in containing.Project.Documents)
                {
                    if (document == containing)
                    {
                        continue;
                    }

                    if (await Check(document).ConfigureAwait(false) is CodeStyleResult documentResult &&
                        documentResult != CodeStyleResult.NotFound)
                    {
                        return documentResult;
                    }
                }

                return CodeStyleResult.NotFound;

                async Task<CodeStyleResult> Check(Document candidate)
                {
                    var tree = await candidate.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                    if (IsExcluded(tree))
                    {
                        return CodeStyleResult.NotFound;
                    }

                    if (tree.TryGetRoot(out var root))
                    {
                        this.Visit(root);
                        return this.result;
                    }

                    return CodeStyleResult.NotFound;
                }
            }

            /// <summary>
            /// Walk <paramref name="containing"/> then trees in <paramref name="compilation"/>.
            /// </summary>
            /// <param name="containing">The <see cref="Document"/>.</param>
            /// <param name="compilation">The <see cref="Compilation"/>.</param>
            /// <returns>The <see cref="CodeStyleResult"/> found.</returns>
            protected CodeStyleResult CheckCore(SyntaxTree containing, Compilation compilation)
            {
                if (Check(containing) is CodeStyleResult containingResult &&
                    containingResult != CodeStyleResult.NotFound)
                {
                    return containingResult;
                }

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (syntaxTree == containing)
                    {
                        continue;
                    }

                    if (Check(syntaxTree) is CodeStyleResult syntaxTreeResult &&
                        syntaxTreeResult != CodeStyleResult.NotFound)
                    {
                        return syntaxTreeResult;
                    }
                }

                return CodeStyleResult.NotFound;

                CodeStyleResult Check(SyntaxTree tree)
                {
                    if (IsExcluded(tree))
                    {
                        return CodeStyleResult.NotFound;
                    }

                    if (tree.TryGetRoot(out var root))
                    {
                        this.Visit(root);
                        return this.result;
                    }

                    return CodeStyleResult.NotFound;
                }
            }

            /// <summary>
            /// Update the result field.
            /// </summary>
            /// <param name="newValue">The <see cref="CodeStyleResult"/>.</param>
            protected void Update(CodeStyleResult newValue)
            {
                switch (this.result)
                {
                    case CodeStyleResult.NotFound:
                        this.result = newValue;
                        break;
                    case CodeStyleResult.Yes:
                        if (newValue == CodeStyleResult.No)
                        {
                            this.result = CodeStyleResult.Mixed;
                        }

                        break;
                    case CodeStyleResult.No:
                        if (newValue == CodeStyleResult.Yes)
                        {
                            this.result = CodeStyleResult.Mixed;
                        }

                        break;
                    case CodeStyleResult.Mixed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
                }
            }

            /// <inheritdoc />
            protected override void Clear()
            {
                this.result = CodeStyleResult.NotFound;
            }
        }

        private sealed class QualifyFieldAccessWalker : CompilationWalker<QualifyFieldAccessWalker>
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
                        this.Update(CodeStyleResult.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                        memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                        IsMemberField():
                        this.Update(CodeStyleResult.Yes);
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

            internal static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyFieldAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class QualifyPropertyAccessWalker : CompilationWalker<QualifyPropertyAccessWalker>
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
                        this.Update(CodeStyleResult.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                        memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                        IsMemberProperty():
                        this.Update(CodeStyleResult.Yes);
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

            internal static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyPropertyAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class QualifyMethodAccessWalker : CompilationWalker<QualifyMethodAccessWalker>
        {
            private QualifyMethodAccessWalker()
            {
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                switch (node.Expression)
                {
                    case IdentifierNameSyntax _ when IsMemberMethod():
                        this.Update(CodeStyleResult.No);
                        break;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression.IsKind(SyntaxKind.ThisExpression):
                        this.Update(CodeStyleResult.Yes);
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

            internal static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new QualifyMethodAccessWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }
        }

        private sealed class UnderscoreFieldWalker : CompilationWalker<UnderscoreFieldWalker>
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
                        this.Update(name.StartsWith("_", StringComparison.Ordinal) ? CodeStyleResult.Yes : CodeStyleResult.No);
                    }
                }
            }

            internal static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new UnderscoreFieldWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }

            internal static CodeStyleResult Check(SyntaxTree containing, Compilation compilation)
            {
                using (var walker = Borrow(() => new UnderscoreFieldWalker()))
                {
                    return walker.CheckCore(containing, compilation);
                }
            }
        }

        private sealed class UsingDirectivesInsideNamespaceWalker : CompilationWalker<UsingDirectivesInsideNamespaceWalker>
        {
            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                this.Update(node.TryFirstAncestor<NamespaceDeclarationSyntax>(out _) ? CodeStyleResult.Yes : CodeStyleResult.No);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
            {
                // Stop walking here
            }

            internal static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
            {
                using (var walker = Borrow(() => new UsingDirectivesInsideNamespaceWalker()))
                {
                    return await walker.CheckCoreAsync(containing, cancellationToken)
                                       .ConfigureAwait(false);
                }
            }

            internal static CodeStyleResult Check(SyntaxTree containing, Compilation compilation)
            {
                using (var walker = Borrow(() => new UsingDirectivesInsideNamespaceWalker()))
                {
                    return walker.CheckCore(containing, compilation);
                }
            }
        }

        private sealed class BackingFieldsAdjacentWalker : CompilationWalker<BackingFieldsAdjacentWalker>
        {
            private bool newLineBetween;

            public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                // Don't walk, optimization.
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                // Don't walk, optimization.
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (node.TryGetBackingField(out var field) &&
                    node.Parent is TypeDeclarationSyntax containingType)
                {
                    if (containingType.Members.IndexOf(node) is int pi &&
                        containingType.Members.IndexOf(field) is int fi &&
                        fi == pi - 1)
                    {
                        if (fi == 0 ||
                            containingType.Members[fi - 1].IsKind(SyntaxKind.FieldDeclaration))
                        {
                            return;
                        }

                        this.newLineBetween = node.HasLeadingTrivia && node.GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia);
                        this.Update(CodeStyleResult.Yes);
                    }
                    else
                    {
                        this.Update(CodeStyleResult.No);
                    }
                }
            }

            internal static CodeStyleResult Check(SyntaxTree containing, Compilation compilation, out bool newLineBetween)
            {
                using (var walker = Borrow(() => new BackingFieldsAdjacentWalker()))
                {
                    var result = walker.CheckCore(containing, compilation);
                    newLineBetween = walker.newLineBetween;
                    return result;
                }
            }

            protected override void Clear()
            {
                this.newLineBetween = false;
                base.Clear();
            }
        }
    }
}
