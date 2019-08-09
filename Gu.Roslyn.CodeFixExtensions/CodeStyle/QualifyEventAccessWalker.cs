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
    public sealed class QualifyEventAccessWalker : CompilationStyleWalker<QualifyEventAccessWalker>
    {
        private QualifyEventAccessWalker()
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
            using (var walker = Borrow(() => new QualifyEventAccessWalker()))
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
            using (var walker = Borrow(() => new QualifyEventAccessWalker()))
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
                case ConditionalAccessExpressionSyntax conditionalAccess when conditionalAccess.Parent.IsKind(SyntaxKind.ExpressionStatement) &&
                                                                              conditionalAccess.WhenNotNull.IsKind(SyntaxKind.InvocationExpression) &&
                                                                              IsMemberEvent():
                    this.Update(CodeStyleResult.No);
                    break;
                case InvocationExpressionSyntax invocation when invocation.Parent.IsKind(SyntaxKind.ExpressionStatement) &&
                                                                IsMemberEvent():
                    this.Update(CodeStyleResult.No);
                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression == node &&
                                                                    memberAccess.Parent is InvocationExpressionSyntax invocation &&
                                                                    invocation.Parent.IsKind(SyntaxKind.ExpressionStatement) &&
                                                                    IsMemberEvent():
                    this.Update(CodeStyleResult.No);
                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Name == node &&
                                                                    memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                    IsMemberEvent():
                    this.Update(CodeStyleResult.Yes);
                    break;
            }

            bool IsMemberEvent()
            {
                return node.TryFirstAncestor(out MemberDeclarationSyntax containingMember) &&
                       !IsStatic(containingMember) &&
                       containingMember.Parent is TypeDeclarationSyntax containingType &&
                       containingType.TryFindEvent(node.Identifier.ValueText, out var @event) &&
                       !IsStatic(@event);

                bool IsStatic(MemberDeclarationSyntax candidate)
                {
                    switch (candidate)
                    {
                        case BaseMethodDeclarationSyntax declaration:
                            return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                        case BasePropertyDeclarationSyntax declaration:
                            return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                        case EventFieldDeclarationSyntax declaration:
                            return declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                        default:
                            return true;
                    }
                }
            }
        }
    }
}
