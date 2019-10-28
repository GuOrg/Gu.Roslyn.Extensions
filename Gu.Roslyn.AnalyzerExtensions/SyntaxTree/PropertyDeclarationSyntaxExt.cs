namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="PropertyDeclarationSyntax"/>.
    /// </summary>
    public static class PropertyDeclarationSyntaxExt
    {
        /// <summary>
        /// Return the backing field from checking what the getter returns.
        /// Assumes
        /// 1. Only one return.
        /// 2. The field can be found in the Parent type declaration so inheritance and partial not handled.
        /// </summary>
        /// <param name="property">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="backingField">The backing field if found.</param>
        /// <returns>True if a backing field was found.</returns>
        public static bool TryGetBackingField(this PropertyDeclarationSyntax property, [NotNullWhen(true)]out FieldDeclarationSyntax? backingField)
        {
            if (property is null)
            {
                backingField = null;
                return false;
            }

            if (TrySingleReturned(property, out var returned) &&
                property.Parent is TypeDeclarationSyntax type)
            {
                switch (returned)
                {
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                        return type.TryFindField(memberAccess.Name.Identifier.ValueText, out backingField);
                    case IdentifierNameSyntax identifierName:
                        return type.TryFindField(identifierName.Identifier.ValueText, out backingField);
                }
            }

            backingField = null;
            return false;
        }

        /// <summary>
        /// Get the single returned value from the getter.
        /// </summary>
        /// <param name="property">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="result">The returned <see cref="ExpressionSyntax"/>.</param>
        /// <returns>True if a single return was found.</returns>
        public static bool TrySingleReturned(this PropertyDeclarationSyntax property, [NotNullWhen(true)]out ExpressionSyntax? result)
        {
            result = null;
            if (property is null)
            {
                return false;
            }

            var expressionBody = property.ExpressionBody;
            if (expressionBody != null)
            {
                result = expressionBody.Expression;
                return result != null;
            }

            if (property.TryGetGetter(out var getter))
            {
                expressionBody = getter.ExpressionBody;
                if (expressionBody != null)
                {
                    result = expressionBody.Expression;
                    return result != null;
                }

                var body = getter.Body;
                if (body is null ||
                    body.Statements.Count == 0)
                {
                    return false;
                }

                if (body.Statements.TrySingle(out var statement) &&
                    statement is ReturnStatementSyntax returnStatement)
                {
                    result = returnStatement.Expression;
                    return result != null;
                }
            }

            return false;
        }
    }
}
