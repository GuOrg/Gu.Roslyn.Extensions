namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ParameterListSyntax"/>.
    /// </summary>
    public static class ParameterListSyntaxExt
    {
        /// <summary>
        /// Get the argument that matches <paramref name="name"/>.
        /// </summary>
        /// <param name="parameterList">The <see cref="ParameterListSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFind(this ParameterListSyntax parameterList, string name, out ParameterSyntax parameter)
        {
            if (parameterList == null ||
                string.IsNullOrEmpty(name))
            {
                parameter = null;
                return false;
            }

            foreach (var candidate in parameterList.Parameters)
            {
                if (candidate.Identifier.ValueText == name)
                {
                    parameter = candidate;
                    return true;
                }
            }

            parameter = null;
            return false;
        }
    }
}
