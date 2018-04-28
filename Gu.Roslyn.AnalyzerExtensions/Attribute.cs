namespace Gu.Roslyn.AnalyzerExtensions
{
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
        /// <param name="attributeLists">The <see cref="AttributeListSyntax"/></param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="result">The match</param>
        /// <returns>True if an attribute of the expected type was found.</returns>
        public static bool TryFind(SyntaxList<AttributeListSyntax> attributeLists, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken, out AttributeSyntax result)
        {
            result = null;
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (IsType(attribute, expected, semanticModel, cancellationToken))
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
        /// <param name="attribute">The <see cref="AttributeSyntax"/></param>
        /// <param name="expected">The expected type.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>True if the attribute is of the expected type.</returns>
        public static bool IsType(AttributeSyntax attribute, QualifiedType expected, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (attribute == null)
            {
                return false;
            }

            if (attribute.Name is SimpleNameSyntax simpleName)
            {
                if (!IsMatch(simpleName, expected) &&
                    !AliasWalker.TryGet(attribute.SyntaxTree, simpleName.Identifier.ValueText, out _))
                {
                    return false;
                }
            }
            else if (attribute.Name is QualifiedNameSyntax qualifiedName &&
                     qualifiedName.Right is SimpleNameSyntax typeName)
            {
                if (!IsMatch(typeName, expected) &&
                    !AliasWalker.TryGet(attribute.SyntaxTree, typeName.Identifier.ValueText, out _))
                {
                    return false;
                }
            }

            return semanticModel.TryGetType(attribute, cancellationToken, out var attributeType) &&
                   attributeType == expected;

            bool IsMatch(SimpleNameSyntax sn, QualifiedType qt)
            {
                return sn.Identifier.ValueText == qt.Type ||
                       qt.Type.IsParts(sn.Identifier.ValueText, "Attribute");
            }
        }

        /// <summary>
        /// Check if the attribute is of the expected type.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/></param>
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
