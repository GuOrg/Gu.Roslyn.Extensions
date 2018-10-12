namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with <see cref="ITypeSymbol"/> when generic.
    /// </summary>
    public class QualifiedGenericType : QualifiedType
    {
        private readonly string metaDataName;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedGenericType"/> class.
        /// </summary>
        /// <param name="fullName">For example 'System.String'</param>
        /// <param name="typeArguments">The type arguments.</param>
        public QualifiedGenericType(string fullName, ImmutableArray<QualifiedType> typeArguments)
            : base(fullName, AliasOrNull(fullName, typeArguments))
        {
            this.metaDataName = fullName?.IndexOf("[") is int i &&
                                i > 0
                ? fullName.Substring(0, i)
                : fullName;

            this.TypeArguments = typeArguments;
        }

        /// <summary>
        /// Gets the type arguments.
        /// </summary>
        public ImmutableArray<QualifiedType> TypeArguments { get; }

        /// <inheritdoc />
        public override INamedTypeSymbol GetTypeSymbol(Compilation compilation)
        {
            return compilation.GetTypeByMetadataName(this.metaDataName).Construct(this.TypeArguments.Select(x => x.GetTypeSymbol(compilation)).ToArray());
        }

        private static string AliasOrNull(string fullName, ImmutableArray<QualifiedType> typeArguments)
        {
            if (typeArguments.TrySingle(out var arg) &&
                fullName == "System.Nullable`1")
            {
                if (TypeAliasMap.TryGetValue(arg.FullName, out var alias))
                {
                    return alias + "?";
                }

                return arg.FullName + "?";
            }

            return null;
        }
    }
}
