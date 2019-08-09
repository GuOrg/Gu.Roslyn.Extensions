namespace Gu.Roslyn.CodeFixExtensions
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public sealed class BackingFieldsAdjacentWalker : CompilationStyleWalker<BackingFieldsAdjacentWalker>
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