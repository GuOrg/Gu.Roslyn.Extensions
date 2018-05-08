namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="FieldDeclarationSyntax"/>
    /// </summary>
    public static class FieldDeclarationSyntaxExt
    {
        /// <summary>
        /// Try getting the name of the field.
        /// </summary>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/></param>
        /// <param name="name">The name</param>
        /// <returns>True if the declaration is for a single variable.</returns>
        public static bool TryGetName(this FieldDeclarationSyntax declaration, out string name)
        {
            name = null;
            if (declaration == null)
            {
                return false;
            }

            if (declaration.Declaration is VariableDeclarationSyntax variableDeclaration &&
                variableDeclaration.Variables.TrySingle(out var variable))
            {
                name = variable.Identifier.ValueText;
            }

            return name != null;
        }

        /// <summary>
        /// Try to find the variable by name.
        /// </summary>
        /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/></param>
        /// <param name="name">The name.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TryFindVariable(this FieldDeclarationSyntax fieldDeclaration, string name, out VariableDeclaratorSyntax result)
        {
            result = null;
            if (fieldDeclaration == null)
            {
                return false;
            }

            foreach (var candidate in fieldDeclaration.Declaration.Variables)
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
