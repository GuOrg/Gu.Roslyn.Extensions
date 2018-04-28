namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A walker that looks for type aliases.
    /// </summary>
    public sealed class AliasWalker : PooledWalker<AliasWalker>
    {
        private readonly List<NameEqualsSyntax> aliases = new List<NameEqualsSyntax>();

        private AliasWalker()
        {
        }

        /// <summary>
        /// Gets the aliases found in the scope.
        /// </summary>
        public IReadOnlyList<NameEqualsSyntax> Aliases => this.aliases;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>
        /// </summary>
        /// <param name="node">The scope</param>
        /// <returns>A walker that has visited <paramref name="node"/></returns>
        public static AliasWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new AliasWalker());

        /// <summary>
        /// Try to get the type alias for the type name.
        /// </summary>
        /// <param name="tree">The <see cref="SyntaxTree"/></param>
        /// <param name="typeName">The type name.</param>
        /// <param name="result">The alias if found.</param>
        /// <returns>True if an alias was found.</returns>
        public static bool TryGet(SyntaxTree tree, string typeName, out NameEqualsSyntax result)
        {
            result = null;
            if (tree == null ||
                typeName == null)
            {
                return false;
            }

            if (tree.TryGetRoot(out var root))
            {
                using (var walker = Borrow(root))
                {
                    foreach (var candidate in walker.aliases)
                    {
                        if (candidate.Name.Identifier.ValueText == typeName)
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
            if (node.Alias != null)
            {
                this.aliases.Add(node.Alias);
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
