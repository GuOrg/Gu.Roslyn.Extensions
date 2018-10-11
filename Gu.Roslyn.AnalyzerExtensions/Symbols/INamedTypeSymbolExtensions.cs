namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for <see cref="INamedTypeSymbol"/>
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
                        builder.Append(part.Symbol.MetadataName);
                        break;
                    case SymbolDisplayPartKind.Punctuation when part.ToString() == ".":
                        builder.Append(previous.Symbol == null || previous.Symbol.Kind == SymbolKind.Namespace ? "." : "+");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
    }
}
