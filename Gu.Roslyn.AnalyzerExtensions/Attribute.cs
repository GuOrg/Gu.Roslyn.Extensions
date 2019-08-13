namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for working with attributes.
    /// </summary>
    public static class Attribute
    {
        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="compilation">The <see cref="CompilationUnitSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(CompilationUnitSyntax compilation, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (compilation is null)
            {
                result = null;
                return false;
            }

            return TryFind(compilation.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="typeDeclaration">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(TypeDeclarationSyntax typeDeclaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (typeDeclaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(typeDeclaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(FieldDeclarationSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(BaseMethodDeclarationSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(EventDeclarationSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(EventFieldDeclarationSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(BasePropertyDeclarationSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="declaration">The <see cref="ParameterSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(ParameterSyntax declaration, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            if (declaration is null)
            {
                result = null;
                return false;
            }

            return TryFind(declaration.AttributeLists, expected, semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try find an attribute of the expected type.
        /// </summary>
        /// <param name="attributeLists">The <see cref="AttributeListSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(this SyntaxList<AttributeListSyntax> attributeLists, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            result = null;
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (semanticModel.TryGetNamedType(attribute, expected, cancellationToken, out _))
                    {
                        result = attribute;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the attribute is of the expected type.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/>.</param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if the attribute is of the expected type.</returns>
        [Obsolete("Prefer semanticModel.TryGetType with QualifiedType specified.")]
        public static bool IsType(AttributeSyntax attribute, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return semanticModel.TryGetNamedType(attribute, expected, cancellationToken, out _);
        }

        /// <summary>
        /// Check if the attribute is of the expected type.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/>.</param>
        /// <param name="name">The type name if found.</param>
        /// <returns>True if the name of the attribute type was found.</returns>
        public static bool TryGetTypeName(AttributeSyntax attribute, out string name)
        {
            name = null;
            if (attribute == null)
            {
                return false;
            }

            if (attribute.Name is SimpleNameSyntax simpleName)
            {
                name = simpleName.Identifier.ValueText;
                return true;
            }

            if (attribute.Name is QualifiedNameSyntax qualifiedName &&
                qualifiedName.Right is SimpleNameSyntax typeName)
            {
                name = typeName.Identifier.ValueText;
                return true;
            }

            return false;
        }
    }
}
