namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public sealed class IdentifierNameExecutionWalker : ExecutionWalker<IdentifierNameExecutionWalker>
    {
        private readonly List<IdentifierNameSyntax> identifierNames = new List<IdentifierNameSyntax>();

        private IdentifierNameExecutionWalker()
        {
        }

        /// <summary>
        /// Gets the <see cref="IdentifierNameSyntax"/>s found in the scope.
        /// </summary>
        public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="scope">The scope</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>A walker that has visited <paramref name="node"/></returns>
        public static IdentifierNameExecutionWalker Borrow(SyntaxNode node, Scope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new IdentifierNameExecutionWalker());
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            this.identifierNames.Add(node);
            base.VisitIdentifierName(node);
        }

        /// <summary>
        /// Try find an <see cref="IdentifierNameSyntax"/> by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="identifierName">The <see cref="IdentifierNameSyntax"/></param>
        /// <returns>True if a match was found</returns>
        public bool TryFind(string name, out IdentifierNameSyntax identifierName)
        {
            foreach (var candidate in this.identifierNames)
            {
                if (candidate.Identifier.ValueText == name)
                {
                    identifierName = candidate;
                    return true;
                }
            }

            identifierName = null;
            return false;
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.identifierNames.Clear();
        }
    }
}
