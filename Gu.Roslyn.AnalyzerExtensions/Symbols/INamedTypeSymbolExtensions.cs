namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for <see cref="INamedTypeSymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class INamedTypeSymbolExtensions
    {
        private static readonly SymbolDisplayFormat Simple = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable);

        /// <summary>
        /// Returns what System.Type.FullName returns.
        /// </summary>
        /// <param name="type">The <see cref="INamedTypeSymbol"/>.</param>
        /// <returns>What System.Type.FullName returns.</returns>
        public static string FullName(this INamedTypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var builder = StringBuilderPool.Borrow();
            var previous = default(SymbolDisplayPart);
            foreach (var part in type.ToDisplayParts(Simple))
            {
                switch (part.Kind)
                {
                    case SymbolDisplayPartKind.ClassName:
                    case SymbolDisplayPartKind.InterfaceName:
                    case SymbolDisplayPartKind.StructName:
                    case SymbolDisplayPartKind.NamespaceName:
                        if (part.Symbol is { } symbol)
                        {
                            builder.Append(symbol.MetadataName);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Part symbol is null {part}.");
                        }

                        break;
                    case SymbolDisplayPartKind.Punctuation when part.ToString() == ".":
                        builder.Append(previous.Symbol is null || previous.Symbol.Kind == SymbolKind.Namespace ? "." : "+");
                        break;
                    default:
                        throw new InvalidOperationException($"Not handling member {part.Kind}.");
                }

                if (part.Symbol != null)
                {
                    previous = part;
                }
            }

            if (type.ConstructedFrom != type)
            {
                builder.Append("[");
                for (var i = 0; i < type.TypeArguments.Length; i++)
                {
                    var argument = type.TypeArguments[i];
                    if (i > 0)
                    {
                        builder.Append(",");
                    }

                    builder.Append("[");
                    if (argument is INamedTypeSymbol argType)
                    {
                        builder.Append(FullName(argType))
                               .Append(", ").Append(argType.ContainingAssembly.Identity.ToString());
                    }

                    builder.Append("]");
                }

                builder.Append("]");
            }

            return builder.Return();
        }

        /// <summary>
        /// Find the first override of <paramref name="virtualOrAbstract"/>.
        /// </summary>
        /// <param name="type">The type to start searching from.</param>
        /// <param name="virtualOrAbstract">The <see cref="IEventSymbol"/>.</param>
        /// <returns>No <see langword="null"/>if an override was found.</returns>
        public static IEventSymbol? FindOverride(this INamedTypeSymbol type, IEventSymbol virtualOrAbstract)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (virtualOrAbstract is null)
            {
                throw new ArgumentNullException(nameof(virtualOrAbstract));
            }

            return type.TryFindEventRecursive(virtualOrAbstract.Name, out var overrider) &&
                   overrider.IsOverride
                ? overrider
                : null;
        }

        /// <summary>
        /// Find the first override of <paramref name="virtualOrAbstract"/>.
        /// </summary>
        /// <param name="type">The type to start searching from.</param>
        /// <param name="virtualOrAbstract">The <see cref="IPropertySymbol"/>.</param>
        /// <returns>No <see langword="null"/>if an override was found.</returns>
        public static IPropertySymbol? FindOverride(this INamedTypeSymbol type, IPropertySymbol virtualOrAbstract)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (virtualOrAbstract is null)
            {
                throw new ArgumentNullException(nameof(virtualOrAbstract));
            }

            return type.TryFindPropertyRecursive(virtualOrAbstract.Name, out var overrider) &&
                   overrider.IsOverride
                ? overrider
                : null;
        }

        /// <summary>
        /// Find the first override of <paramref name="virtualOrAbstract"/>.
        /// </summary>
        /// <param name="type">The type to start searching from.</param>
        /// <param name="virtualOrAbstract">The <see cref="IMethodSymbol"/>.</param>
        /// <returns>No <see langword="null"/>if an override was found.</returns>
        public static IMethodSymbol? FindOverride(this INamedTypeSymbol type, IMethodSymbol virtualOrAbstract)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (virtualOrAbstract is null)
            {
                throw new ArgumentNullException(nameof(virtualOrAbstract));
            }

            return type.TryFindFirstMethodRecursive(virtualOrAbstract.Name, x => x.IsOverride && Matches(x), out var overrider)
                ? overrider
                : null;

            bool Matches(IMethodSymbol candidate)
            {
                if (!candidate.ReturnType.Equals(virtualOrAbstract.ReturnType) ||
                    candidate.Parameters.Length != virtualOrAbstract.Parameters.Length ||
                    candidate.TypeParameters.Length != virtualOrAbstract.TypeParameters.Length)
                {
                    return false;
                }

                for (var i = 0; i < candidate.Parameters.Length; i++)
                {
                    if (!candidate.Parameters[i].Type.Equals(virtualOrAbstract.Parameters[i].Type))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
