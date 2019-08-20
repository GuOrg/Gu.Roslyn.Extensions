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
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            switch (node.Parent)
            {
                case InvocationExpressionSyntax _:
                case ArgumentSyntax argument when argument.Parent is ArgumentListSyntax argumentList &&
                                                  argumentList.Parent is InvocationExpressionSyntax invocation &&
                                                  invocation.IsNameOf():
                    if (IsInstanceMethod() &&
                        !Scope.HasLocal(node, node.Identifier.ValueText) &&
                        !Scope.HasParameter(node, node.Identifier.ValueText))
                    {
                        this.Update(CodeStyleResult.No);
                    }

                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Name == node &&
                                                                    memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                    IsInstanceMethod():
                    this.Update(CodeStyleResult.Yes);
                    break;
            }

            bool IsInstanceMethod()
            {
                return !node.IsInStaticContext() &&
                       node.TryFirstAncestor(out TypeDeclarationSyntax containingType) &&
                       containingType.TryFindMethod(node.Identifier.Text, out var method) &&
                       !method.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                       !containingType.TryFindMethod(node.Identifier.Text, x => x.Modifiers.Any(SyntaxKind.StaticKeyword), out _) &&
                       !Scope.HasLocal(node, node.Identifier.ValueText) &&
                       !Scope.HasParameter(node, node.Identifier.ValueText);
            }
        }
    }
}
