namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="VariableDeclarationSyntax"/>.
    /// </summary>
    public static class VariableDeclarationSyntaxExt
    {
        /// <summary>
        /// Try to find the variable by name.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclarationSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TryFindVariable(this VariableDeclarationSyntax variableDeclaration, string name, out VariableDeclaratorSyntax result)
        {
            result = null;
            if (variableDeclaration is null)
            {
                return false;
            }

            foreach (var candidate in variableDeclaration.Variables)
            {
                if (candidate.Identifier.ValueText == name)
                {
                    result = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
