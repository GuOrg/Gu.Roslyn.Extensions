﻿namespace Gu.Roslyn.CodeFixExtensions;

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
        if (compilationUnit is null)
        {
            throw new System.ArgumentNullException(nameof(compilationUnit));
        }

        if (type is null)
        {
            throw new System.ArgumentNullException(nameof(type));
        }

        if (semanticModel is null)
        {
            throw new System.ArgumentNullException(nameof(semanticModel));
        }

        var updated = AddUsing(compilationUnit, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(type.ContainingNamespace.ToDisplayString())), semanticModel);
        if (type is INamedTypeSymbol { IsGenericType: true } genericType)
        {
            foreach (var argument in genericType.TypeArguments)
            {
                updated = AddUsing(updated, argument, semanticModel);
            }
        }

        return updated;
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
        if (compilationUnit is null)
        {
            throw new System.ArgumentNullException(nameof(compilationUnit));
        }

        if (semanticModel is null)
        {
            throw new System.ArgumentNullException(nameof(semanticModel));
        }

        if (usingDirective is null)
        {
            throw new System.ArgumentNullException(nameof(usingDirective));
        }

        if (compilationUnit.Members.TrySingleOfType<MemberDeclarationSyntax, NamespaceDeclarationSyntax>(out var ns) &&
            UsingDirectiveComparer.IsSameOrContained(ns, usingDirective))
        {
            return compilationUnit;
        }

        using var walker = UsingDirectiveWalker.Borrow(compilationUnit);
        if (walker.UsingDirectives.Count == 0)
        {
            if (walker.NamespaceDeclarations.TryFirst(out var namespaceDeclaration))
            {
                if (CodeStyle.UsingDirectivesInsideNamespace(semanticModel) != CodeStyleResult.No)
                {
                    return compilationUnit.ReplaceNode(namespaceDeclaration, namespaceDeclaration.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                }

                return compilationUnit.ReplaceNode(compilationUnit, compilationUnit.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
            }

            if (compilationUnit.Members.TryFirst(out var first) &&
                first is FileScopedNamespaceDeclarationSyntax fsns)
            {
                if (CodeStyle.UsingDirectivesInsideNamespace(semanticModel) == CodeStyleResult.Yes)
                {
                    return compilationUnit.ReplaceNode(compilationUnit, compilationUnit.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                }

                return compilationUnit.ReplaceNode(fsns, fsns.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
            }

            return compilationUnit;
        }

        UsingDirectiveSyntax? previous = null;
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

        if (previous is null)
        {
            throw new InvalidOperationException("Did not find node to insert after.");
        }

        return compilationUnit.InsertNodesAfter(previous, new[] { usingDirective.WithTrailingElasticLineFeed() });
    }

    private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
    {
        private readonly List<UsingDirectiveSyntax> usingDirectives = new();
        private readonly List<NamespaceDeclarationSyntax> namespaceDeclarations = new();

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
