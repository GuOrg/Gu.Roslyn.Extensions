namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper class with factory methods for <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.
    /// </summary>
    public static class SymbolAndDeclaration
    {
        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{IFieldSymbol, FieldDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IFieldSymbol, FieldDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(FieldDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IFieldSymbol, FieldDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
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
        /// <param name="declaration">The <see cref="ConstructorDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, ConstructorDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(ConstructorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, ConstructorDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
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
        /// <param name="declaration">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IEventSymbol, EventDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(EventDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IEventSymbol, EventDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<IEventSymbol, EventDeclarationSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{IEventSymbol, EventFieldDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IEventSymbol, EventFieldDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(EventFieldDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IEventSymbol, EventFieldDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
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
        /// <param name="declaration">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IPropertySymbol, PropertyDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(PropertyDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IPropertySymbol, PropertyDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<IPropertySymbol, PropertyDeclarationSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{IMethodSymbol, AccessorDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="AccessorDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, PropertAccessorDeclarationSyntaxyDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(AccessorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, AccessorDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out IMethodSymbol symbol))
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
        /// <param name="declaration">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IMethodSymbol, MethodDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(MethodDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IMethodSymbol, MethodDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
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
        /// <param name="declaration">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{ITypeSymbol, TypeDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(TypeDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<ITypeSymbol, TypeDeclarationSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<ITypeSymbol, TypeDeclarationSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{ITypeSymbol, EnumDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{ITypeSymbol, EnumDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(EnumDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, EnumDeclarationSyntax> result)
        {
            if (semanticModel.TryGetNamedType(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<INamedTypeSymbol, EnumDeclarationSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{ITypeSymbol, StructDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="StructDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{ITypeSymbol, StructDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(StructDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, StructDeclarationSyntax> result)
        {
            if (semanticModel.TryGetNamedType(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<INamedTypeSymbol, StructDeclarationSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Create a <see cref="SymbolAndDeclaration{ITypeSymbol,ClassDeclarationSyntax}"/> is symbol exists.
        /// </summary>
        /// <param name="declaration">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{ITypeSymbol, ClassDeclarationSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(ClassDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<INamedTypeSymbol, ClassDeclarationSyntax> result)
        {
            if (semanticModel.TryGetNamedType(declaration, cancellationToken, out var symbol))
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
        /// <param name="declaration">The <see cref="ParameterSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="result">The <see cref="SymbolAndDeclaration{IParameterSymbol, ParameterSyntax}"/>.</param>
        /// <returns>True if the symbol exists.</returns>
        public static bool Create(ParameterSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken, out SymbolAndDeclaration<IParameterSymbol, ParameterSyntax> result)
        {
            if (semanticModel.TryGetSymbol(declaration, cancellationToken, out var symbol))
            {
                result = new SymbolAndDeclaration<IParameterSymbol, ParameterSyntax>(symbol, declaration);
                return true;
            }

            result = default;
            return false;
        }
    }
}
