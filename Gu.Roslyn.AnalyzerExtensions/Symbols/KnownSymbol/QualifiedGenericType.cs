namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
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
            : base(fullName, NamespaceParts.Create(fullName), TypeName(fullName), null)
        {
            if (fullName?.IndexOf("[") is int i &&
                i > 0)
            {
                this.metaDataName = fullName.Substring(0, i);
            }
            else
            {
                throw new InvalidOperationException("Expected a name like: System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]\r\n" +
                                                    "The name type.FullName returns.");
            }

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

        private static string TypeName(string fullName)
        {
            var end = fullName.IndexOf('[');
            var start = fullName.LastIndexOf('.', end) + 1;
            return fullName.Substring(start, end - start);
        }
    }
}
