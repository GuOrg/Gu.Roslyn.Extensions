namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
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

            if (other == KnownSymbol.System.Object)
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
                   NamedTypeSymbolComparer.Equals(firstNamed, otherNamed);
        }

        public static bool IsSameType(this INamedTypeSymbol first, INamedTypeSymbol other)
        {
            if (first == null ||
                other == null)
            {
                return false;
            }

            if (first.IsDefinition ^ other.IsDefinition)
            {
                return IsSameType(first.OriginalDefinition, other.OriginalDefinition);
            }

            return first.Equals(other) ||
                   AreEquivalent(first, other);
        }

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

            var typeInfo = semanticModel.GetTypeInfoSafe(value, cancellationToken);
            return namedTypeSymbol.TypeArguments[0].IsSameType(typeInfo.Type);
        }

        [Obsolete("Use type.TypeKind == TypeKind.Interface")]
        public static bool IsInterface(this ITypeSymbol type)
        {
            return type.TypeKind == TypeKind.Interface;
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
