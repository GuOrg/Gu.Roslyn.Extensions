namespace Gu.Roslyn.CodeFixExtensions
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Figure out if the current style is to have backing fields immediately above before the property.
    /// </summary>
    public sealed class BackingFieldsAdjacentWalker : CompilationStyleWalker<BackingFieldsAdjacentWalker>
    {
        private bool newLineBetween;

        /// <summary>
        /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
        /// </summary>
        /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
        /// <param name="compilation">The current <see cref="Compilation"/>.</param>
        /// <param name="newLineBetween">If there is a newline between the field and the property.</param>
        /// <returns>The <see cref="CodeStyleResult"/>.</returns>
        public static CodeStyleResult Check(SyntaxTree containing, Compilation compilation, out bool newLineBetween)
        {
            using (var walker = Borrow(() => new BackingFieldsAdjacentWalker()))
            {
                var result = walker.CheckCore(containing, compilation);
                newLineBetween = walker.newLineBetween;
                return result;
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
            // Don't walk, optimization.
        }

        /// <inheritdoc />
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (!node.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                node.TryGetBackingField(out var field) &&
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

        /// <inheritdoc />
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // Don't walk, optimization.
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.newLineBetween = false;
            base.Clear();
        }
    }
}
