// ReSharper disable UnusedMember.Global
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    public static class TypeDeclarationSyntaxExt
    {
        /// <summary>
        /// Find a <see cref="FieldDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindField(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)] out FieldDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (var member in type.Members)
            {
                if (member is FieldDeclarationSyntax declaration &&
                    declaration.TryFindVariable(name, out _))
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindConstructor(this TypeDeclarationSyntax type, [NotNullWhen(true)] out ConstructorDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (var member in type.Members)
            {
                if (member is ConstructorDeclarationSyntax declaration)
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindConstructor(this TypeDeclarationSyntax type, Func<ConstructorDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out ConstructorDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var member in type.Members)
            {
                if (member is ConstructorDeclarationSyntax declaration &&
                    predicate(declaration))
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="EventDeclarationSyntax"/> or <see cref="EventFieldDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindEvent(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)] out MemberDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (var member in type.Members)
            {
                switch (member)
                {
                    case EventDeclarationSyntax { Identifier: { ValueText: { } valueText } } declaration
                        when valueText == name:
                        match = declaration;
                        return true;
                    case EventFieldDeclarationSyntax { Declaration: { Variables: { } variables } } eventField
                        when variables.TrySingle(x => x.Identifier.ValueText == name, out _):
                        match = eventField;
                        return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="PropertyDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindProperty(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)] out PropertyDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (var member in type.Members)
            {
                if (member is PropertyDeclarationSyntax { Identifier: { ValueText: { } valueText } } declaration &&
                    valueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="IndexerDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindIndexer(this TypeDeclarationSyntax type, [NotNullWhen(true)] out IndexerDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (var member in type.Members)
            {
                if (member is IndexerDeclarationSyntax declaration)
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindMethod(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)] out MethodDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax { Identifier: { ValueText: { } valueText } } declaration &&
                    valueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindMethod(this TypeDeclarationSyntax type, string name, Func<MethodDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out MethodDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax { Identifier: { ValueText: { } valueText } } declaration &&
                    valueText == name &&
                    predicate(declaration))
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindMethod(this TypeDeclarationSyntax type, Func<MethodDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out MethodDeclarationSyntax? match)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax declaration &&
                    predicate(declaration))
                {
                    match = declaration;
                    return true;
                }
            }

            match = null;
            return false;
        }
    }
}
