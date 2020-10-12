namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
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
        public static bool TryGetBackingField(this PropertyDeclarationSyntax property, [NotNullWhen(true)] out FieldDeclarationSyntax? backingField)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (TrySingleReturned(property, out var returned) &&
                property.Parent is TypeDeclarationSyntax type)
            {
                switch (returned)
                {
                    case MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax _, Name: { Identifier: { ValueText: { } name } } }:
                        return type.TryFindField(name, out backingField);
                    case IdentifierNameSyntax { Identifier: { ValueText: { } name } }:
                        return type.TryFindField(name, out backingField);
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
        public static bool TrySingleReturned(this PropertyDeclarationSyntax property, [NotNullWhen(true)] out ExpressionSyntax? result)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            switch (property)
            {
                case { ExpressionBody: { Expression: { } expression } }:
                    result = expression;
                    return true;
            }

            if (property.TryGetGetter(out var getter))
            {
                switch (getter)
                {
                    case { ExpressionBody: { Expression: { } expression } }:
                        result = expression;
                        return true;
                    case { Body: { Statements: { Count: 1 } statements } }
                        when statements.TrySingle(out var statement) &&
                             statement is ReturnStatementSyntax { Expression: { } expression }:
                        result = expression;
                        return true;
                }
            }

            result = null;
            return false;
        }
    }
}
