namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for finding members of <see cref="ITypeSymbol"/> or base types.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ITypeSymbolExt
    {
        /// <summary>
        /// Try finding the <see cref="IFieldSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="field">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFieldRecursive(this ITypeSymbol type, string name, out IFieldSymbol field)
        {
            return type.TryFindFirstMemberRecursive(name, out field);
        }

        /// <summary>
        /// Try finding the <see cref="IEventSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="event">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindEventRecursive(this ITypeSymbol type, string name, out IEventSymbol @event)
        {
            return type.TryFindFirstMemberRecursive(name, out @event);
        }

        /// <summary>
        /// Try finding the <see cref="IPropertySymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="property">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindPropertyRecursive(this ITypeSymbol type, string name, out IPropertySymbol property)
        {
            if (name == "Item[]")
            {
                return type.TryFindFirstMemberRecursive(x => x.IsIndexer, out property);
            }

            return type.TryFindFirstMemberRecursive(name, out property);
        }

        /// <summary>
        /// Try finding the <see cref="IMethodSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethodRecursive(this ITypeSymbol type, string name, out IMethodSymbol result)
        {
            return type.TryFindFirstMemberRecursive(name, out result);
        }

        /// <summary>
        /// Try finding the <see cref="IMethodSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethodRecursive(this ITypeSymbol type, Func<IMethodSymbol, bool> predicate, out IMethodSymbol result)
        {
            return type.TryFindFirstMemberRecursive(predicate, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethodRecursive(this ITypeSymbol type, string name, out IMethodSymbol result)
        {
            return type.TryFindSingleMemberRecursive(name, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethodRecursive(this ITypeSymbol type, Func<IMethodSymbol, bool> predicate, out IMethodSymbol result)
        {
            return type.TryFindSingleMemberRecursive(predicate, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethodRecursive(this ITypeSymbol type, string name, Func<IMethodSymbol, bool> predicate, out IMethodSymbol result)
        {
            return type.TryFindSingleMemberRecursive(name, predicate, out result);
        }

        /// <summary>
        /// Try finding the first matching <see cref="IMethodSymbol"/>.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethodRecursive(this ITypeSymbol type, string name, Func<IMethodSymbol, bool> predicate, out IMethodSymbol result)
        {
            return type.TryFindFirstMemberRecursive(name, predicate, out result);
        }

        /// <summary>
        /// Try finding the first member by name.
        /// Look in <paramref name="type"/> and base types.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMemberRecursive(this ITypeSymbol type, string name, out ISymbol result)
        {
            return type.TryFindFirstMemberRecursive<ISymbol>(name, out result);
        }

        private static bool TryFindSingleMemberRecursive<TMember>(this ITypeSymbol type, string name, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null ||
                string.IsNullOrEmpty(name))
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers(name))
                {
                    if (member != null)
                    {
                        member = null;
                        return false;
                    }

                    member = symbol as TMember;
                }

                if (member?.IsOverride == true)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return member != null;
        }

        private static bool TryFindSingleMemberRecursive<TMember>(this ITypeSymbol type, Func<TMember, bool> predicate, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null ||
                predicate == null)
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers())
                {
                    if (symbol is TMember candidate &&
                        predicate(candidate))
                    {
                        if (member != null)
                        {
                            member = null;
                            return false;
                        }

                        member = candidate;
                    }
                }

                type = type.BaseType;
            }

            return member != null;
        }

        private static bool TryFindSingleMemberRecursive<TMember>(this ITypeSymbol type, string name, Func<TMember, bool> predicate, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null ||
                predicate == null)
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers(name))
                {
                    if (symbol is TMember candidate &&
                        predicate(candidate))
                    {
                        if (member != null)
                        {
                            member = null;
                            return false;
                        }

                        member = candidate;
                    }
                }

                type = type.BaseType;
            }

            return member != null;
        }

        private static bool TryFindFirstMemberRecursive<TMember>(this ITypeSymbol type, Func<TMember, bool> predicate, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null ||
                predicate == null)
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers())
                {
                    if (symbol is TMember candidate &&
                        predicate(candidate))
                    {
                        member = candidate;
                        return true;
                    }
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool TryFindFirstMemberRecursive<TMember>(this ITypeSymbol type, string name, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null)
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers(name))
                {
                    if (symbol is TMember candidate)
                    {
                        member = candidate;
                        return true;
                    }
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool TryFindFirstMemberRecursive<TMember>(this ITypeSymbol type, string name, Func<TMember, bool> predicate, out TMember member)
            where TMember : class, ISymbol
        {
            member = null;
            if (type == null ||
                predicate == null)
            {
                return false;
            }

            while (type != null)
            {
                foreach (var symbol in type.GetMembers(name))
                {
                    if (symbol is TMember candidate &&
                        predicate(candidate))
                    {
                        member = candidate;
                        return true;
                    }
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
