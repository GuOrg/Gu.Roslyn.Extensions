namespace Gu.Roslyn.AnalyzerExtensions;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Extension methods for <see cref="BaseObjectCreationExpressionSyntax"/>.
/// </summary>
public static class BaseObjectCreationExpressionSyntaxExt
{
    /// <summary>
    /// Get the argument that matches <paramref name="parameter"/>.
    /// </summary>
    /// <param name="invocation">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
    /// <returns><see cref="ArgumentSyntax"/> if a match was found.</returns>
    public static ArgumentSyntax? FindArgument(this BaseObjectCreationExpressionSyntax invocation, IParameterSymbol parameter)
    {
        return TryFindArgument(invocation, parameter, out var match) ? match : null;
    }

    /// <summary>
    /// Get the argument that matches <paramref name="parameter"/>.
    /// </summary>
    /// <param name="objectCreation">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
    /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
    /// <returns>True if a match was found.</returns>
    public static bool TryFindArgument(this BaseObjectCreationExpressionSyntax objectCreation, IParameterSymbol parameter, [NotNullWhen(true)] out ArgumentSyntax? argument)
    {
        if (objectCreation is null)
        {
            throw new System.ArgumentNullException(nameof(objectCreation));
        }

        if (parameter is null)
        {
            throw new System.ArgumentNullException(nameof(parameter));
        }

        argument = null;
        return objectCreation.ArgumentList is { } argumentList &&
              argumentList.TryFind(parameter, out argument);
    }

    /// <summary>
    /// Get the argument that matches <paramref name="parameter"/>.
    /// </summary>
    /// <param name="creation">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
    /// <param name="arguments">The <see cref="ImmutableArray{ArgumentSyntax}"/>.</param>
    /// <returns>True if one or more were found.</returns>
    public static bool TryFindArgumentParams(this BaseObjectCreationExpressionSyntax creation, IParameterSymbol parameter, out ImmutableArray<ArgumentSyntax> arguments)
    {
        if (creation is null)
        {
            throw new System.ArgumentNullException(nameof(creation));
        }

        if (parameter is null)
        {
            throw new System.ArgumentNullException(nameof(parameter));
        }

        arguments = default;
        return creation.ArgumentList is { } argumentList &&
               argumentList.TryFindParams(parameter, out arguments);
    }

    /// <summary>
    /// Try getting the declaration of the invoked method.
    /// </summary>
    /// <param name="node">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns><see cref="ConstructorDeclarationSyntax"/> if the declaration was found.</returns>
    public static ConstructorDeclarationSyntax? TargetDeclaration(this BaseObjectCreationExpressionSyntax node, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        return TryGetTargetDeclaration(node, semanticModel, cancellationToken, out var declaration) ? declaration : null;
    }

    /// <summary>
    /// Try getting the declaration of the invoked method.
    /// </summary>
    /// <param name="node">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <param name="declaration">The <see cref="ConstructorDeclarationSyntax"/>.</param>
    /// <returns>True if the declaration was found.</returns>
    public static bool TryGetTargetDeclaration(this BaseObjectCreationExpressionSyntax node, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out ConstructorDeclarationSyntax? declaration)
    {
        if (node is null)
        {
            throw new System.ArgumentNullException(nameof(node));
        }

        if (semanticModel is null)
        {
            throw new System.ArgumentNullException(nameof(semanticModel));
        }

        declaration = null;
        return semanticModel.TryGetSymbol(node, cancellationToken, out var symbol) &&
               symbol.TrySingleDeclaration(cancellationToken, out declaration);
    }

    /// <summary>
    /// Check if <paramref name="node"/> is <paramref name="type"/>.
    /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
    /// </summary>
    /// <param name="node">The <see cref="BaseObjectCreationExpressionSyntax"/>.</param>
    /// <param name="type">The <see cref="QualifiedType"/>.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>True  if <paramref name="node"/> is <paramref name="type"/>.</returns>
    public static bool IsType(this BaseObjectCreationExpressionSyntax node, QualifiedType type, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        return node switch
        {
            ObjectCreationExpressionSyntax { Type: IdentifierNameSyntax identifierName } => identifierName == type &&
                                                                                            semanticModel.TryGetType(node, cancellationToken, out var symbol) &&
                                                                                            symbol == type,
            _ => semanticModel.TryGetType(node, cancellationToken, out var symbol) &&
                 symbol == type,
        };
    }
}
