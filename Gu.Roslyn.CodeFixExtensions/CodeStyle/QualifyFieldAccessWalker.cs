namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Figure out if this.Name is the current style.
    /// </summary>
    public sealed class QualifyFieldAccessWalker : CompilationStyleWalker<QualifyFieldAccessWalker>
    {
        private QualifyFieldAccessWalker()
        {
        }

        /// <summary>
        /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
        /// </summary>
        /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>The <see cref="CodeStyleResult"/>.</returns>
        public static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
        {
            using var walker = Borrow(() => new QualifyFieldAccessWalker());
            return await walker.CheckCoreAsync(containing, cancellationToken)
                               .ConfigureAwait(false);
        }

        /// <summary>
        /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
        /// </summary>
        /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
        /// <param name="compilation">The current <see cref="Compilation"/>.</param>
        /// <returns>The <see cref="CodeStyleResult"/>.</returns>
        public static CodeStyleResult Check(SyntaxTree containing, Compilation compilation)
        {
            using var walker = Borrow(() => new QualifyFieldAccessWalker());
            return walker.CheckCore(containing, compilation);
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            switch (node.Parent)
            {
                case AssignmentExpressionSyntax assignment when assignment.Left.Contains(node) &&
                                                                !assignment.Parent.IsKind(SyntaxKind.ObjectInitializerExpression):
                case ArrowExpressionClauseSyntax _:
                case ReturnStatementSyntax _:
                case ArgumentSyntax _:
                    if (IsInstanceField() &&
                        !Scope.HasLocal(node, node.Identifier.ValueText) &&
                        !Scope.HasParameter(node, node.Identifier.ValueText))
                    {
                        this.Update(CodeStyleResult.No);
                    }

                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Contains(node) &&
                                                                    memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                    IsInstanceField():
                    this.Update(CodeStyleResult.Yes);
                    break;
            }

            bool IsInstanceField()
            {
                return !node.IsInStaticContext() &&
                       node.TryFirstAncestor(out TypeDeclarationSyntax? containingType) &&
                       containingType.TryFindField(node.Identifier.ValueText, out var field) &&
                       !field.Modifiers.Any(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword);
            }
        }

        /// <inheritdoc />
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // don't walk
        }

        /// <inheritdoc />
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                base.VisitConstructorDeclaration(node);
            }
        }

        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                base.VisitClassDeclaration(node);
            }
        }

        /// <inheritdoc />
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                base.VisitStructDeclaration(node);
            }
        }

        /// <inheritdoc />
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                base.VisitPropertyDeclaration(node);
            }
        }

        /// <inheritdoc />
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                base.VisitMethodDeclaration(node);
            }
        }
    }
}
