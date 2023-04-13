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
        /// <returns><see cref="FieldDeclarationSyntax"/> if a match was found.</returns>
        public static FieldDeclarationSyntax? FindField(this TypeDeclarationSyntax type, string name)
        {
            return TryFindField(type, name, out var match) ? match : null;
        }

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
        /// Find the first <see cref="FieldDeclarationSyntax"/> matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindField(this TypeDeclarationSyntax type, Func<FieldDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out FieldDeclarationSyntax? match)
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
                if (member is FieldDeclarationSyntax declaration &&
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
        /// Find the first <see cref="ConstructorDeclarationSyntax"/>.
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
        /// Find a <see cref="MemberDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <returns><see cref="MemberDeclarationSyntax"/> if a match was found.</returns>
        public static MemberDeclarationSyntax? FindEvent(this TypeDeclarationSyntax type, string name)
        {
            return TryFindEvent(type, name, out var match) ? match : null;
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
                    case EventDeclarationSyntax { Identifier.ValueText: { } valueText } declaration
                        when valueText == name:
                        match = declaration;
                        return true;
                    case EventFieldDeclarationSyntax { Declaration.Variables: { } variables } eventField
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
        /// <returns><see cref="PropertyDeclarationSyntax"/> if a match was found.</returns>
        public static PropertyDeclarationSyntax? FindProperty(this TypeDeclarationSyntax type, string name)
        {
            return TryFindProperty(type, name, out var match) ? match : null;
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
                if (member is PropertyDeclarationSyntax { Identifier.ValueText: { } valueText } declaration &&
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
        /// Find the first <see cref="PropertyDeclarationSyntax"/> matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindProperty(this TypeDeclarationSyntax type, Func<PropertyDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out PropertyDeclarationSyntax? match)
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
                if (member is PropertyDeclarationSyntax declaration &&
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
        /// Find the first <see cref="IndexerDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <returns><see cref="IndexerDeclarationSyntax"/> if a match was found.</returns>
        public static IndexerDeclarationSyntax? FindIndexer(this TypeDeclarationSyntax type, Func<IndexerDeclarationSyntax, bool> predicate)
        {
            return TryFindIndexer(type, predicate, out var match) ? match : null;
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
        /// Find a <see cref="IndexerDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindIndexer(this TypeDeclarationSyntax type, Func<IndexerDeclarationSyntax, bool> predicate, [NotNullWhen(true)] out IndexerDeclarationSyntax? match)
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
                if (member is IndexerDeclarationSyntax declaration &&
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
        /// Find the first <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <returns><see cref="MethodDeclarationSyntax"/> if a match was found.</returns>
        public static MethodDeclarationSyntax? FindMethod(this TypeDeclarationSyntax type, string name)
        {
            return TryFindMethod(type, name, out var match) ? match : null;
        }

        /// <summary>
        /// Find the first <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="predicate">The filter.</param>
        /// <returns><see cref="MethodDeclarationSyntax"/> if a match was found.</returns>
        public static MethodDeclarationSyntax? FindMethod(this TypeDeclarationSyntax type, string name, Func<MethodDeclarationSyntax, bool> predicate)
        {
            return TryFindMethod(type, name, predicate, out var match) ? match : null;
        }

        /// <summary>
        /// Find the first <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <returns><see cref="MethodDeclarationSyntax"/> if a match was found.</returns>
        public static MethodDeclarationSyntax? FindMethod(this TypeDeclarationSyntax type, Func<MethodDeclarationSyntax, bool> predicate)
        {
            return TryFindMethod(type, predicate, out var match) ? match : null;
        }

        /// <summary>
        /// Find the first <see cref="MethodDeclarationSyntax"/> by name.
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
                if (member is MethodDeclarationSyntax { Identifier.ValueText: { } valueText } declaration &&
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
                if (member is MethodDeclarationSyntax { Identifier.ValueText: { } valueText } declaration &&
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
