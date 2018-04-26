namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ITypeSymbol"/>
    /// </summary>
    public static partial class TypeSymbolExt
    {
        /// <summary>
        /// Check if <paramref name="type"/> is <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/>
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/></param>
        /// <param name="qualifiedType1">The first <see cref="QualifiedType"/></param>
        /// <param name="qualifiedType2">The second <see cref="QualifiedType"/></param>
        /// <returns>True if <paramref name="type"/> is <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/></returns>
        public static bool IsEither(this ITypeSymbol type, QualifiedType qualifiedType1, QualifiedType qualifiedType2) => type == qualifiedType1 || type == qualifiedType2;

        /// <summary>
        /// Check if <paramref name="type"/> is <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/> or <paramref name="qualifiedType3"/>
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/></param>
        /// <param name="qualifiedType1">The first <see cref="QualifiedType"/></param>
        /// <param name="qualifiedType2">The second <see cref="QualifiedType"/></param>
        /// <param name="qualifiedType3">The third <see cref="QualifiedType"/></param>
        /// <returns>True if <paramref name="type"/> is <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/> or <paramref name="qualifiedType3"/></returns>
        public static bool IsEither(this ITypeSymbol type, QualifiedType qualifiedType1, QualifiedType qualifiedType2, QualifiedType qualifiedType3) => type == qualifiedType1 || type == qualifiedType2 || type == qualifiedType3;

        /// <summary>
        /// Check if <paramref name="type"/> is <paramref name="qualifiedType"/>
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/></param>
        /// <param name="qualifiedType">The <see cref="QualifiedType"/></param>
        /// <returns>True if <paramref name="type"/> is <paramref name="qualifiedType"/> </returns>
        public static bool Is(this ITypeSymbol type, QualifiedType qualifiedType)
        {
            if (type == null || qualifiedType == null)
            {
                return false;
            }

            while (type != null)
            {
                if (type == qualifiedType)
                {
                    return true;
                }

                foreach (var @interface in type.AllInterfaces)
                {
                    if (@interface == qualifiedType)
                    {
                        return true;
                    }
                }

                type = type.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Check if <paramref name="type"/> is <paramref name="other"/>
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/></param>
        /// <param name="other">The other <see cref="ITypeSymbol"/></param>
        /// <returns>True if <paramref name="type"/> is <paramref name="other"/> </returns>
        public static bool Is(this ITypeSymbol type, ITypeSymbol other)
        {
            if (type == null || other == null)
            {
                return false;
            }

            if (other == QualifiedType.System.Object)
            {
                return true;
            }

            if (other.TypeKind == TypeKind.Interface)
            {
                foreach (var @interface in type.AllInterfaces)
                {
                    if (IsSameType(@interface, other))
                    {
                        return true;
                    }
                }

                return false;
            }

            while (type?.BaseType != null)
            {
                if (IsSameType(type, other))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        /// <summary> Check if <paramref name="x"/> is the same type as <paramref name="y"/> </summary>
        /// <param name="x">The first type.</param>
        /// <param name="y">The other type.</param>
        /// <returns>True if same type.</returns>
        public static bool IsSameType(this ITypeSymbol x, ITypeSymbol y)
        {
            if (Equals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x is ITypeParameterSymbol firstParameter &&
                y is ITypeParameterSymbol otherParameter)
            {
                return firstParameter.MetadataName == otherParameter.MetadataName &&
                       firstParameter.ContainingSymbol.Equals(otherParameter.ContainingSymbol);
            }

            return x is INamedTypeSymbol firstNamed &&
                   y is INamedTypeSymbol otherNamed &&
                   IsSameType(firstNamed, otherNamed);
        }

        /// <summary> Check if <paramref name="x"/> is the same type as <paramref name="y"/> </summary>
        /// <param name="x">The first type.</param>
        /// <param name="y">The other type.</param>
        /// <returns>True if same type.</returns>
        public static bool IsSameType(this INamedTypeSymbol x, INamedTypeSymbol y)
        {
            if (x == null ||
                y == null)
            {
                return false;
            }

            if (x.IsDefinition ^ y.IsDefinition)
            {
                return IsSameType(x.OriginalDefinition, y.OriginalDefinition);
            }

            return x.Equals(y) ||
                   AreEquivalent(x, y);
        }

        /// <summary>
        /// Check if <paramref name="value"/> can be assigned to <paramref name="nullableType"/>
        /// </summary>
        /// <param name="nullableType">The <see cref="ITypeSymbol"/></param>
        /// <param name="value">The <see cref="ExpressionSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>True if <paramref name="value"/> can be assigned to <paramref name="nullableType"/></returns>
        public static bool IsNullable(this ITypeSymbol nullableType, ExpressionSyntax value, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var namedTypeSymbol = nullableType as INamedTypeSymbol;
            if (namedTypeSymbol == null ||
                !namedTypeSymbol.IsGenericType ||
                namedTypeSymbol.Name != "Nullable" ||
                namedTypeSymbol.TypeParameters.Length != 1)
            {
                return false;
            }

            if (value.IsKind(SyntaxKind.NullLiteralExpression))
            {
                return true;
            }

            return semanticModel.TryGetTypeInfo(value, cancellationToken, out ITypeSymbol type) &&
                   type is INamedTypeSymbol namedType &&
                   namedType.TypeArguments.TrySingle(out var typeArg) &&
                   IsSameType(typeArg, nullableType);
        }

        private static bool AreEquivalent(this INamedTypeSymbol first, INamedTypeSymbol other)
        {
            if (ReferenceEquals(first, other))
            {
                return true;
            }

            if (first == null ||
                other == null)
            {
                return false;
            }

            if (first.MetadataName != other.MetadataName ||
                first.ContainingModule.MetadataName != other.ContainingModule.MetadataName ||
                first.Arity != other.Arity)
            {
                return false;
            }

            for (var i = 0; i < first.Arity; i++)
            {
                if (!IsSameType(first.TypeArguments[i], other.TypeArguments[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
