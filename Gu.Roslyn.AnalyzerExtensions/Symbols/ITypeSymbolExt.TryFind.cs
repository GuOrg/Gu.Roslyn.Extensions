namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for finding members of <see cref="ITypeSymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ITypeSymbolExt
    {
        /// <summary>
        /// Try finding the <see cref="IFieldSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="field">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindField(this ITypeSymbol type, string name, [NotNullWhen(true)] out IFieldSymbol? field)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.TryFindSingleMember(name, out field);
        }

        /// <summary>
        /// Try finding the <see cref="IEventSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="event">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindEvent(this ITypeSymbol type, string name, [NotNullWhen(true)] out IEventSymbol? @event)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.TryFindFirstMember(name, out @event);
        }

        /// <summary>
        /// Try finding the <see cref="IPropertySymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="property">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindProperty(this ITypeSymbol type, string name, [NotNullWhen(true)] out IPropertySymbol? property)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            if (name == "Item[]")
            {
                return type.TryFindFirstMember(x => x.IsIndexer, out property);
            }

            return type.TryFindFirstMember(name, out property);
        }

        /// <summary>
        /// Try finding the <see cref="IMethodSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethod(this ITypeSymbol type, string name, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.TryFindFirstMember(name, out result);
        }

        /// <summary>
        /// Try finding the <see cref="IMethodSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethod(this ITypeSymbol type, Func<IMethodSymbol, bool> predicate, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return type.TryFindFirstMember(predicate, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethod(this ITypeSymbol type, string name, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.TryFindSingleMember(name, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethod(this ITypeSymbol type, Func<IMethodSymbol, bool> predicate, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return type.TryFindSingleMember(predicate, out result);
        }

        /// <summary>
        /// Try finding the only <see cref="IMethodSymbol"/> by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMethod(this ITypeSymbol type, string name, Func<IMethodSymbol, bool> predicate, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return type.TryFindSingleMember(name, predicate, out result);
        }

        /// <summary>
        /// Try finding the first matching <see cref="IMethodSymbol"/>.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="predicate">The func to filter by.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMethod(this ITypeSymbol type, string name, Func<IMethodSymbol, bool> predicate, [NotNullWhen(true)] out IMethodSymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return type.TryFindFirstMember(name, predicate, out result);
        }

        /// <summary>
        /// Try finding the first member by name.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMember(this ITypeSymbol type, string name, [NotNullWhen(true)] out ISymbol? result)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.TryFindFirstMember<ISymbol>(name, out result);
        }

        /// <summary>
        /// Try finding the first member by predicate.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMember<TMember>(this ITypeSymbol type, string name, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            member = null;
            if (name.Length == 0)
            {
                return false;
            }

            return type.GetMembers(name).TrySingleOfType<ISymbol, TMember>(out member);
        }

        /// <summary>
        /// Try finding the first member by predicate.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMember<TMember>(this ITypeSymbol type, Func<TMember, bool> predicate, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            member = null;

            foreach (var symbol in type.GetMembers())
            {
                if (symbol is TMember candidate &&
                    predicate(candidate))
                {
                    if (member is { })
                    {
                        member = null;
                        return false;
                    }

                    member = candidate;
                }
            }

            return member is { };
        }

        /// <summary>
        /// Try finding the single member by name and predicate.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindSingleMember<TMember>(this ITypeSymbol type, string name, Func<TMember, bool> predicate, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            member = null;
            foreach (var symbol in type.GetMembers(name))
            {
                if (symbol is TMember candidate &&
                    predicate(candidate))
                {
                    if (member is { })
                    {
                        member = null;
                        return false;
                    }

                    member = candidate;
                }
            }

            return member is { };
        }

        /// <summary>
        /// Try finding the first member by predicate.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMember<TMember>(this ITypeSymbol type, Func<TMember, bool> predicate, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            member = null;
            foreach (var symbol in type.GetMembers())
            {
                if (symbol is TMember candidate &&
                    predicate(candidate))
                {
                    member = candidate;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Try finding the first member by name.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMember<TMember>(this ITypeSymbol type, string name, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            return type.GetMembers(name).TryFirstOfType<ISymbol, TMember>(out member);
        }

        /// <summary>
        /// Try finding the first member by predicate.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="type">The containing type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="predicate">The filter.</param>
        /// <param name="member">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindFirstMember<TMember>(this ITypeSymbol type, string name, Func<TMember, bool> predicate, [NotNullWhen(true)] out TMember? member)
            where TMember : class, ISymbol
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            member = null;
            foreach (var symbol in type.GetMembers(name))
            {
                if (symbol is TMember candidate &&
                    predicate(candidate))
                {
                    member = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
