namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for simplifying qualified type names.
    /// </summary>
    public static class Simplify
    {
        /// <summary>
        /// Walks the node and adds Simplifier.Annotation to all <see cref="QualifiedNameSyntax"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="node">The node.</param>
        /// <returns>The node with simplifier annotations.</returns>
        public static T WithSimplifiedNames<T>(this T node)
            where T : SyntaxNode
        {
            return (T)SimplifyNamesRewriter.Default.Visit(node);
        }
    }
}
