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
        public static bool TryFindField(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)]out FieldDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
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

            return false;
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/>.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindConstructor(this TypeDeclarationSyntax type, [NotNullWhen(true)]out ConstructorDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is ConstructorDeclarationSyntax declaration)
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find a <see cref="ConstructorDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindConstructor(this TypeDeclarationSyntax type, Func<ConstructorDeclarationSyntax, bool> predicate, [NotNullWhen(true)]out ConstructorDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
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

            return false;
        }

        /// <summary>
        /// Find a <see cref="EventDeclarationSyntax"/> or <see cref="EventFieldDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindEvent(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)]out MemberDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                switch (member)
                {
                    case EventDeclarationSyntax declaration when declaration.Identifier.ValueText == name:
                        match = declaration;
                        return true;
                    case EventFieldDeclarationSyntax eventField when eventField.Declaration.Variables.TrySingle(x => x.Identifier.ValueText == name, out _):
                        match = eventField;
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find a <see cref="PropertyDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindProperty(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)]out PropertyDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is PropertyDeclarationSyntax declaration &&
                    declaration.Identifier.ValueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find a <see cref="IndexerDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindIndexer(this TypeDeclarationSyntax type, [NotNullWhen(true)]out IndexerDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is IndexerDeclarationSyntax declaration)
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindMethod(this TypeDeclarationSyntax type, string name, [NotNullWhen(true)]out MethodDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax declaration &&
                    declaration.Identifier.ValueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

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
        public static bool TryFindMethod(this TypeDeclarationSyntax type, string name, Func<MethodDeclarationSyntax, bool> predicate, [NotNullWhen(true)]out MethodDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax declaration &&
                    declaration.Identifier.ValueText == name &&
                    predicate(declaration))
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find a <see cref="MethodDeclarationSyntax"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="match">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindMethod(this TypeDeclarationSyntax type, Func<MethodDeclarationSyntax, bool> predicate, [NotNullWhen(true)]out MethodDeclarationSyntax? match)
        {
            match = null;
            if (type is null)
            {
                return false;
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

            return false;
        }
    }
}
