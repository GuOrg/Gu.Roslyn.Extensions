namespace Gu.Roslyn.CodeFixExtensions
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Rewrite <see cref="ClassDeclarationSyntax"/> to sealed.
    /// Change protected -> private
    /// Remove virtual.
    /// </summary>
    public sealed class SealRewriter : CSharpSyntaxRewriter
    {
        private static readonly SealRewriter Default = new SealRewriter();

        /// <summary>
        /// Rewrite <paramref name="classDeclaration"/> to sealed.
        /// Change protected -> private
        /// Remove virtual.
        /// </summary>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="ClassDeclarationSyntax"/> that was passed in rewritten as sealed.</returns>
        public static ClassDeclarationSyntax Seal(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration is null)
            {
                throw new System.ArgumentNullException(nameof(classDeclaration));
            }

            return Default.SealCore(classDeclaration);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // We only want to make the top level class sealed.
            return node;
        }

        /// <inheritdoc />
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (TryUpdate(node.Modifiers, out var modifiers))
            {
                return node.WithModifiers(modifiers);
            }

            return node;
        }

        /// <inheritdoc />
        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (TryUpdate(node.Modifiers, out var modifiers))
            {
                node = node.WithModifiers(modifiers);
            }

            return base.VisitEventDeclaration(node);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return node;
            }

            if (TryUpdate(node.Modifiers, out var modifiers))
            {
                node = node.WithModifiers(modifiers);
            }

            return base.VisitPropertyDeclaration(node);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.TryFirstAncestor(out BasePropertyDeclarationSyntax? parent) &&
                parent.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                node.Modifiers.TrySingle(x => x.IsKind(SyntaxKind.PrivateKeyword), out var modifier))
            {
                return node.WithModifiers(node.Modifiers.Remove(modifier));
            }

            return TryUpdate(node.Modifiers, out var modifiers)
                ? node.WithModifiers(modifiers)
                : node;
        }

        /// <inheritdoc />
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return node;
            }

            if (TryUpdate(node.Modifiers, out var modifiers))
            {
                return node.WithModifiers(modifiers);
            }

            return node;
        }

        private static bool TryUpdate(SyntaxTokenList modifiers, out SyntaxTokenList result)
        {
            result = modifiers;
            if (modifiers.TrySingle(x => x.IsKind(SyntaxKind.VirtualKeyword), out var modifier))
            {
                result = modifiers.Remove(modifier);
            }

            if (result.TrySingle(x => x.IsKind(SyntaxKind.ProtectedKeyword), out modifier) &&
                !result.Any(SyntaxKind.OverrideKeyword))
            {
                result = result.Replace(modifier, SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            }

            return result != modifiers;
        }

        private ClassDeclarationSyntax SealCore(ClassDeclarationSyntax classDeclaration)
        {
            if (!classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword))
            {
                if (classDeclaration.Modifiers.IndexOf(SyntaxKind.PartialKeyword) is var index &&
                    index >= 0)
                {
                    classDeclaration = classDeclaration.WithModifiers(classDeclaration.Modifiers.Insert(index, SyntaxFactory.Token(SyntaxKind.SealedKeyword)));
                }
                else
                {
                    classDeclaration = classDeclaration.WithModifiers(classDeclaration.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.SealedKeyword)));
                }
            }

            return (ClassDeclarationSyntax)base.VisitClassDeclaration(classDeclaration);
        }
    }
}
