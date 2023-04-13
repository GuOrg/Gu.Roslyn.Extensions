namespace Gu.Roslyn.AnalyzerExtensions;

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Helper class with factory methods for <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.
/// </summary>
public static partial class SymbolAndDeclaration
{
    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IFieldSymbol, FieldDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IFieldSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IFieldSymbol, FieldDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IFieldSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IFieldSymbol, FieldDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleDeclaration(cancellationToken, out var declaration))
        {
            result = new SymbolAndDeclaration<IFieldSymbol, FieldDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IMethodSymbol, ConstructorDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IMethodSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, ConstructorDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IMethodSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.MethodKind == MethodKind.Constructor &&
            symbol.TrySingleDeclaration(cancellationToken, out ConstructorDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IEventSymbol, EventDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IEventSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IEventSymbol, EventDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IEventSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IEventSymbol, EventDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleEventDeclaration(cancellationToken, out var declaration))
        {
            result = new SymbolAndDeclaration<IEventSymbol, EventDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IEventSymbol, EventDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IEventSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IEventSymbol, EventFieldDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IEventSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IEventSymbol, EventFieldDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleEventFieldDeclaration(cancellationToken, out var declaration))
        {
            result = new SymbolAndDeclaration<IEventSymbol, EventFieldDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IPropertySymbol, PropertyDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IPropertySymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IPropertySymbol, BasePropertyDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IPropertySymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IPropertySymbol, BasePropertyDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleDeclaration(cancellationToken, out var declaration))
        {
            result = new SymbolAndDeclaration<IPropertySymbol, BasePropertyDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IMethodSymbol, AccessorDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IMethodSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, AccessorDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IMethodSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, AccessorDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.MethodKind != MethodKind.Ordinary &&
            symbol.TrySingleDeclaration(cancellationToken, out AccessorDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<IMethodSymbol, AccessorDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IMethodSymbol, MethodDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IMethodSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, MethodDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IMethodSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, MethodDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.MethodKind == MethodKind.Ordinary &&
            symbol.TrySingleDeclaration(cancellationToken, out MethodDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<IMethodSymbol, MethodDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{ITypeSymbol, TypeDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="ITypeSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{ITypeSymbol, TypeDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(ITypeSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<ITypeSymbol, TypeDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleDeclaration(cancellationToken, out TypeDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<ITypeSymbol, TypeDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{INamedTypeSymbol, EnumDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="INamedTypeSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{INamedTypeSymbol, EnumDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(INamedTypeSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, EnumDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TypeKind == TypeKind.Enum &&
            symbol.TrySingleDeclaration(cancellationToken, out EnumDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<INamedTypeSymbol, EnumDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{INamedTypeSymbol, StructDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="INamedTypeSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{INamedTypeSymbol, StructDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(INamedTypeSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, StructDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TypeKind == TypeKind.Struct &&
            symbol.TrySingleDeclaration(cancellationToken, out StructDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<INamedTypeSymbol, StructDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{INamedTypeSymbol, ClassDeclarationSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="INamedTypeSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{INamedTypeSymbol, ClassDeclarationSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(INamedTypeSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, ClassDeclarationSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TypeKind == TypeKind.Class &&
            symbol.TrySingleDeclaration(cancellationToken, out ClassDeclarationSyntax? declaration))
        {
            result = new SymbolAndDeclaration<INamedTypeSymbol, ClassDeclarationSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Create a <see cref="SymbolAndDeclaration{IParameterSymbol, ParameterSyntax}"/> is symbol exists.
    /// </summary>
    /// <param name="symbol">The <see cref="IParameterSymbol"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <param name="result">The <see cref="SymbolAndDeclaration{IParameterSymbol, ParameterSyntax}"/>.</param>
    /// <returns>True if the symbol exists.</returns>
    public static bool TryCreate(IParameterSymbol symbol, CancellationToken cancellationToken, out SymbolAndDeclaration<IParameterSymbol, ParameterSyntax> result)
    {
        if (symbol is null)
        {
            throw new System.ArgumentNullException(nameof(symbol));
        }

        if (symbol.TrySingleDeclaration(cancellationToken, out ParameterSyntax? declaration))
        {
            result = new SymbolAndDeclaration<IParameterSymbol, ParameterSyntax>(symbol, declaration);
            return true;
        }

        result = default;
        return false;
    }
}
