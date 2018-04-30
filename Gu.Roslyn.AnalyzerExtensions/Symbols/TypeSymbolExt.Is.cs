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
        /// Check if <paramref name="type"/> is assignable to <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/>
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/></param>
        /// <param name="qualifiedType1">The first <see cref="QualifiedType"/></param>
        /// <param name="qualifiedType2">The second <see cref="QualifiedType"/></param>
        /// <param name="compilation">The <see cref="Compilation"/></param>
        /// <returns>True if <paramref name="type"/> is assignable to <paramref name="qualifiedType1"/> or <paramref name="qualifiedType2"/></returns>
        public static bool IsAssignableToEither(this ITypeSymbol type, QualifiedType qualifiedType1, QualifiedType qualifiedType2, Compilation compilation)
        {
            return type.IsAssignableTo(qualifiedType1, compilation) ||
                   type.IsAssignableTo(qualifiedType2, compilation);
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/></param>
        /// <param name="compilation">The <see cref="Compilation"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="destination"/> </returns>
        public static bool IsAssignableTo(this ITypeSymbol source, ITypeSymbol destination, Compilation compilation)
        {
            if (source == null || destination == null)
            {
                return false;
            }

            return compilation.ClassifyConversion(source, destination).IsImplicit;
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="destination">The <see cref="QualifiedType"/></param>
        /// <param name="compilation">The <see cref="Compilation"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="destination"/> </returns>
        public static bool IsAssignableTo(this ITypeSymbol source, QualifiedType destination, Compilation compilation)
        {
            return IsAssignableTo(source, destination.GetTypeSymbol(compilation), compilation);
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/></param>
        /// <param name="compilation">The <see cref="Compilation"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="destination"/> </returns>
        public static bool IsSameType(this ITypeSymbol source, ITypeSymbol destination, Compilation compilation)
        {
            if (source == null || destination == null)
            {
                return false;
            }

            return compilation.ClassifyConversion(source, destination).IsIdentity;
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="destination">The other <see cref="QualifiedType"/></param>
        /// <param name="compilation">The <see cref="Compilation"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="destination"/> </returns>
        public static bool IsSameType(this ITypeSymbol source, QualifiedType destination, Compilation compilation)
        {
            if (source == null || destination == null)
            {
                return false;
            }

            return IsSameType(source, destination.GetTypeSymbol(compilation), compilation);
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="qualifiedType"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="qualifiedType">The <see cref="QualifiedType"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="qualifiedType"/> </returns>
        [Obsolete("Use IsAssignableTo")]
        public static bool Is(this ITypeSymbol source, QualifiedType qualifiedType)
        {
            if (source == null || qualifiedType == null)
            {
                return false;
            }

            if (source is ITypeParameterSymbol typeParameterSymbol)
            {
                foreach (var constraintType in typeParameterSymbol.ConstraintTypes)
                {
                    if (Is(constraintType, qualifiedType))
                    {
                        return true;
                    }
                }

                return false;
            }

            foreach (var @interface in source.AllInterfaces)
            {
                if (@interface == qualifiedType)
                {
                    return true;
                }
            }

            while (source != null)
            {
                if (source == qualifiedType)
                {
                    return true;
                }

                source = source.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Check if <paramref name="source"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The <see cref="ITypeSymbol"/></param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/></param>
        /// <returns>True if <paramref name="source"/> is <paramref name="destination"/> </returns>
        [Obsolete("Use IsAssignableTo")]
        public static bool Is(this ITypeSymbol source, ITypeSymbol destination)
        {
            if (source == null || destination == null)
            {
                return false;
            }

            if (destination == QualifiedType.System.Object ||
                IsSameType(source, destination))
            {
                return true;
            }

            if (destination == QualifiedType.System.NullableOfT)
            {
                return destination is INamedTypeSymbol nullable &&
                       nullable.TypeArguments.TrySingle(out var arg) &&
                       IsSameType(source, arg);
            }

            foreach (var @interface in source.AllInterfaces)
            {
                if (IsSameType(@interface, destination))
                {
                    return true;
                }
            }

            while (source?.BaseType != null)
            {
                if (IsSameType(source, destination))
                {
                    return true;
                }

                source = source.BaseType;
            }

            return false;
        }

        /// <summary> Check if <paramref name="x"/> is the same type as <paramref name="y"/> </summary>
        /// <param name="x">The first type.</param>
        /// <param name="y">The other type.</param>
        /// <returns>True if same type.</returns>
        [Obsolete("Use IsAssignableTo")]
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

            if (x is ITypeParameterSymbol xtp)
            {
                if (y is ITypeParameterSymbol ytp)
                {
                    return xtp.MetadataName == ytp.MetadataName &&
                           SymbolComparer.Equals(xtp.ContainingSymbol, ytp.ContainingSymbol);
                }

                foreach (var constraintType in xtp.ConstraintTypes)
                {
                    if (Is(constraintType, y))
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (y is ITypeParameterSymbol ytp)
            {
                foreach (var constraintType in ytp.ConstraintTypes)
                {
                    if (Is(x, constraintType))
                    {
                        return true;
                    }
                }

                return false;
            }

            return x is INamedTypeSymbol firstNamed &&
                   y is INamedTypeSymbol otherNamed &&
                   IsSameType(firstNamed, otherNamed);
        }

        /// <summary> Check if <paramref name="x"/> is the same type as <paramref name="y"/> </summary>
        /// <param name="x">The first type.</param>
        /// <param name="y">The other type.</param>
        /// <returns>True if same type.</returns>
        [Obsolete("Use IsAssignableTo")]
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

            return NamedTypeSymbolComparer.Equals(x, y);
        }

        /// <summary>
        /// Check if <paramref name="value"/> can be assigned to <paramref name="nullableType"/>
        /// </summary>
        /// <param name="nullableType">The <see cref="ITypeSymbol"/></param>
        /// <param name="value">The <see cref="ExpressionSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>True if <paramref name="value"/> can be assigned to <paramref name="nullableType"/></returns>
        [Obsolete("Use IsAssignableTo, candidate for removal")]
        public static bool IsNullable(this ITypeSymbol nullableType, ExpressionSyntax value, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (nullableType == null ||
                value == null)
            {
                return false;
            }

            return nullableType is INamedTypeSymbol namedType &&
                   IsNullable(namedType, value, semanticModel, cancellationToken);
        }

        /// <summary>
        /// Check if <paramref name="value"/> can be assigned to <paramref name="nullableType"/>
        /// </summary>
        /// <param name="nullableType">The <see cref="ITypeSymbol"/></param>
        /// <param name="value">The <see cref="ExpressionSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>True if <paramref name="value"/> can be assigned to <paramref name="nullableType"/></returns>
        [Obsolete("Use IsAssignableTo, candidate for removal")]
        public static bool IsNullable(this INamedTypeSymbol nullableType, ExpressionSyntax value, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (nullableType == null ||
                value == null)
            {
                return false;
            }

            if (nullableType.IsGenericType &&
                nullableType.Name == "Nullable" &&
                nullableType.TypeArguments.TrySingle(out var typeArg))
            {
                if (value.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    return true;
                }

                return semanticModel.TryGetType(value, cancellationToken, out var type) &&
                       IsSameType(typeArg, type);
            }

            return false;
        }
    }
}
