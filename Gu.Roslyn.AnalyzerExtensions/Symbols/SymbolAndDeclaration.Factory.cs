namespace Gu.Roslyn.AnalyzerExtensions;

using Microsoft.CodeAnalysis;

/// <summary>
/// Factory methods.
/// </summary>
public static partial class SymbolAndDeclaration
{
    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IFieldSymbol, FieldDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <typeparam name="TSymbol">The type of <paramref name="symbol"/>.</typeparam>
    /// <typeparam name="TDeclaration">The type of <paramref name="declaration"/>.</typeparam>
    /// <param name="symbol">The <typeparamref name="TSymbol"/>.</param>
    /// <param name="declaration">The <typeparamref name="TDeclaration"/>.</param>
    /// <returns>A The <see cref="SymbolAndDeclaration{TSymbol, TDeclaration}"/>.</returns>
    public static SymbolAndDeclaration<TSymbol, TDeclaration> Create<TSymbol, TDeclaration>(TSymbol symbol, TDeclaration declaration)
        where TSymbol : class, ISymbol
        where TDeclaration : SyntaxNode
    {
        return new SymbolAndDeclaration<TSymbol, TDeclaration>(symbol, declaration);
    }
}
