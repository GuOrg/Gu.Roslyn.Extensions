namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A walker that looks for type aliases.
    /// </summary>
    public sealed class AliasWalker : AbstractUsingDirectiveWalker<AliasWalker>
    {
        private readonly List<UsingDirectiveSyntax> aliases = new List<UsingDirectiveSyntax>();

        private AliasWalker()
        {
        }

        /// <summary>
        /// Gets the aliases found in the scope.
        /// </summary>
        public IReadOnlyList<UsingDirectiveSyntax> Aliases => this.aliases;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static AliasWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new AliasWalker());

        /// <summary>
        /// Get a walker that has visited <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree">The scope.</param>
        /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
        public static AliasWalker Borrow(SyntaxTree tree)
        {
            if (tree == null)
            {
                throw new System.ArgumentNullException(nameof(tree));
            }

            if (tree.TryGetRoot(out var root))
            {
                return BorrowAndVisit(root, () => new AliasWalker());
            }

            return Borrow(() => new AliasWalker());
        }

        /// <summary>
        /// Try to get the type alias for the type name.
        /// </summary>
        /// <param name="tree">The <see cref="SyntaxTree"/>.</param>
        /// <param name="name">The type name. using Name = System.String.</param>
        /// <param name="result">The alias if found.</param>
        /// <returns>True if an alias was found.</returns>
        public static bool TryGet(SyntaxTree tree, string name, out UsingDirectiveSyntax result)
        {
            result = null;
            if (tree == null ||
                name == null)
            {
                return false;
            }

            if (tree.TryGetRoot(out var root))
            {
                using (var walker = Borrow(root))
                {
                    foreach (var candidate in walker.aliases)
                    {
                        if (candidate.Alias.Name.Identifier.ValueText == name)
                        {
                            result = candidate;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Try to get the type alias for the type name.
        /// </summary>
        /// <param name="tree">The <see cref="SyntaxTree"/>.</param>
        /// <param name="type">The type name. using Name = System.Type.</param>
        /// <param name="result">The alias if found.</param>
        /// <returns>True if an alias was found.</returns>
        public static bool TryGet(SyntaxTree tree, QualifiedType type, out UsingDirectiveSyntax result)
        {
            result = null;
            if (tree == null ||
                type == null)
            {
                return false;
            }

            if (tree.TryGetRoot(out var root))
            {
                using (var walker = Borrow(root))
                {
                    foreach (var candidate in walker.aliases)
                    {
                        if (candidate.Name == type)
                        {
                            result = candidate;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <inheritdoc />
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node?.Alias != null)
            {
                this.aliases.Add(node);
            }
        }

        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
        }

        /// <inheritdoc />
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.aliases.Clear();
        }
    }
}
