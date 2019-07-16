namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="CompilationUnitSyntax"/>.
    /// </summary>
    public static class CompilationUintSyntaxExtensions
    {
        /// <summary>
        /// Add a using directive for accessing <paramref name="type"/> if needed.
        /// </summary>
        /// <param name="compilationUnit">The <see cref="CompilationUnitSyntax"/>.</param>
        /// <param name="type">The <see cref="ITypeSymbol"/> that needs to be brought into scope.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>The updated <see cref="CompilationUnitSyntax"/>.</returns>
        public static CompilationUnitSyntax AddUsing(this CompilationUnitSyntax compilationUnit, ITypeSymbol type, SemanticModel semanticModel)
        {
            if (compilationUnit == null)
            {
                throw new System.ArgumentNullException(nameof(compilationUnit));
            }

            if (type == null)
            {
                throw new System.ArgumentNullException(nameof(type));
            }

            if (semanticModel == null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            return AddUsing(compilationUnit, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(type.ContainingNamespace.ToDisplayString())), semanticModel);
        }

        /// <summary>
        /// Add a using directive.
        /// </summary>
        /// <param name="compilationUnit">The <see cref="CompilationUnitSyntax"/>.</param>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>The updated <see cref="CompilationUnitSyntax"/>.</returns>
        public static CompilationUnitSyntax AddUsing(this CompilationUnitSyntax compilationUnit, UsingDirectiveSyntax usingDirective, SemanticModel semanticModel)
        {
            if (compilationUnit == null)
            {
                throw new System.ArgumentNullException(nameof(compilationUnit));
            }

            if (semanticModel == null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (usingDirective == null)
            {
                throw new System.ArgumentNullException(nameof(usingDirective));
            }

            if (compilationUnit.Members.TrySingleOfType(out NamespaceDeclarationSyntax ns) &&
                UsingDirectiveComparer.IsSameOrContained(ns, usingDirective))
            {
                return compilationUnit;
            }

            using (var walker = UsingDirectiveWalker.Borrow(compilationUnit))
            {
                if (walker.UsingDirectives.Count == 0)
                {
                    if (walker.NamespaceDeclarations.TryFirst(out var namespaceDeclaration))
                    {
                        if (CodeStyle.UsingDirectivesInsideNamespace(semanticModel))
                        {
                            return compilationUnit.ReplaceNode(namespaceDeclaration, namespaceDeclaration.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                        }

                        return compilationUnit.ReplaceNode(compilationUnit, compilationUnit.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                    }

                    return compilationUnit;
                }

                UsingDirectiveSyntax previous = null;
                foreach (var directive in walker.UsingDirectives)
                {
                    var compare = UsingDirectiveComparer.Compare(directive, usingDirective);
                    if (compare == 0)
                    {
                        return compilationUnit;
                    }

                    if (compare > 0)
                    {
                        return compilationUnit.InsertNodesBefore(directive, new[] { usingDirective.WithTrailingElasticLineFeed() });
                    }

                    previous = directive;
                }

                return compilationUnit.InsertNodesAfter(previous, new[] { usingDirective.WithTrailingElasticLineFeed() });
            }
        }

        private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
        {
            private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();
            private readonly List<NamespaceDeclarationSyntax> namespaceDeclarations = new List<NamespaceDeclarationSyntax>();

            internal IReadOnlyList<UsingDirectiveSyntax> UsingDirectives => this.usingDirectives;

            internal IReadOnlyList<NamespaceDeclarationSyntax> NamespaceDeclarations => this.namespaceDeclarations;

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                this.usingDirectives.Add(node);
            }

            public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                this.namespaceDeclarations.Add(node);
                base.VisitNamespaceDeclaration(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // Stop walking here
            }

            internal static UsingDirectiveWalker Borrow(CompilationUnitSyntax compilationUnit) => BorrowAndVisit(compilationUnit, () => new UsingDirectiveWalker());

            protected override void Clear()
            {
                this.usingDirectives.Clear();
                this.namespaceDeclarations.Clear();
            }
        }
    }
}
