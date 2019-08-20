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
    public sealed class QualifyPropertyAccessWalker : CompilationStyleWalker<QualifyPropertyAccessWalker>
    {
        private QualifyPropertyAccessWalker()
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
            using (var walker = Borrow(() => new QualifyPropertyAccessWalker()))
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
            using (var walker = Borrow(() => new QualifyPropertyAccessWalker()))
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
                                                                !assignment.Parent.IsKind(SyntaxKind.ObjectInitializerExpression):
                case ArrowExpressionClauseSyntax _:
                case ReturnStatementSyntax _:
                case ArgumentSyntax _:
                    if (IsInstanceProperty() &&
                        !Scope.HasLocal(node, node.Identifier.ValueText) &&
                        !Scope.HasParameter(node, node.Identifier.ValueText))
                    {
                        this.Update(CodeStyleResult.No);
                    }

                    break;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Name == node &&
                                                                    memberAccess.Expression.IsKind(SyntaxKind.ThisExpression) &&
                                                                    IsInstanceProperty():
                    this.Update(CodeStyleResult.Yes);
                    break;
            }

            bool IsInstanceProperty()
            {
                return !node.IsInStaticContext() &&
                       node.TryFirstAncestor(out TypeDeclarationSyntax containingType) &&
                       containingType.TryFindProperty(node.Identifier.ValueText, out var property) &&
                       !property.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                       !Scope.HasLocal(node, node.Identifier.ValueText) &&
                       !Scope.HasParameter(node, node.Identifier.ValueText);
            }
        }
    }
}
