namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Immutable;
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
    /// <param name="fullName">For example 'System.String'.</param>
    /// <param name="typeArguments">The type arguments.</param>
    public QualifiedGenericType(string fullName, ImmutableArray<QualifiedType> typeArguments)
        : base(
              fullName ?? throw new ArgumentNullException(nameof(fullName)),
              NamespaceParts.Create(fullName),
              TypeName(fullName),
              null)
    {
        if (fullName.IndexOf("[", StringComparison.Ordinal) is int i &&
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
    public override ITypeSymbol? GetTypeSymbol(Compilation compilation)
    {
        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        if (compilation.GetTypeByMetadataName(this.metaDataName) is { } typeDef &&
            TryGetTypeArguments(out var args))
        {
            return typeDef.Construct(args);
        }

        return null;

        bool TryGetTypeArguments(out ITypeSymbol[] args)
        {
            args = new ITypeSymbol[this.TypeArguments.Length];
            for (var i = 0; i < this.TypeArguments.Length; i++)
            {
                if (this.TypeArguments[i].GetTypeSymbol(compilation) is { } arg)
                {
                    args[i] = arg;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    private static string TypeName(string fullName)
    {
        var end = fullName.IndexOf('[');
        var start = fullName.LastIndexOf('.', end) + 1;
        return fullName.Substring(start, end - start);
    }
}
