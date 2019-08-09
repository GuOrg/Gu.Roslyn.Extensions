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
    public sealed class QualifyMethodAccessWalker : CompilationStyleWalker<QualifyMethodAccessWalker>
    {
        private QualifyMethodAccessWalker()
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
            using (var walker = Borrow(() => new QualifyMethodAccessWalker()))
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
            using (var walker = Borrow(() => new QualifyMethodAccessWalker()))
            {
                return walker.CheckCore(containing, compilation);
            }
        }

        /// <inheritdoc />
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            switch (node.Expression)
            {
                case IdentifierNameSyntax _ when IsMemberMethod():
                    this.Update(CodeStyleResult.No);
                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression.IsKind(SyntaxKind.ThisExpression):
                    this.Update(CodeStyleResult.Yes);
                    break;
            }

            base.VisitInvocationExpression(node);

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
        }
    }
}
