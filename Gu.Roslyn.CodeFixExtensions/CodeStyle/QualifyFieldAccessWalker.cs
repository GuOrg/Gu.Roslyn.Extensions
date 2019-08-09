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
            using (var walker = Borrow(() => new QualifyFieldAccessWalker()))
            {
                return await walker.CheckCoreAsync(containing, cancellationToken)
                                   .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
        /// </summary>
        /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
        /// <param name="compilation">The current <see cref="Compilation"/>.</param>
        /// <returns>The <see cref="CodeStyleResult"/>.</returns>
        public static CodeStyleResult Check(SyntaxTree containing, Compilation compilation)
        {
            using (var walker = Borrow(() => new QualifyFieldAccessWalker()))
            {
                return walker.CheckCore(containing, compilation);
            }
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

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
    }
}
