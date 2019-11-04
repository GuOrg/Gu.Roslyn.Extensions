#pragma warning disable 660,661 // using a hack with operator overloads
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="ISymbol"/>.
    /// </summary>
    /// <typeparam name="T">The type of symbol.</typeparam>
    [DebuggerDisplay("{ContainingType.FullName,nq}.{Name,nq}")]
    public abstract class QualifiedMember<T>
        where T : ISymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedMember{T}"/> class.
        /// </summary>
        /// <param name="containingType">The containing type.</param>
        /// <param name="name">The name.</param>
        protected QualifiedMember(QualifiedType containingType, string name)
        {
            this.Name = name;
            this.ContainingType = containingType;
        }

        /// <summary>
        /// Gets the name of the symbol.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the containing type.
        /// </summary>
        public QualifiedType ContainingType { get; }

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/>. </summary>
        /// <param name="left">The <typeparamref name="T"/>.</param>
        /// <param name="right">The <see cref="QualifiedMember{T}"/>.</param>
        /// <returns>True if found equal.</returns>
        public static bool operator ==(T left, QualifiedMember<T> right)
        {
            return right?.Equals(left) == true;
        }

        /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/>. </summary>
        /// <param name="left">The <typeparamref name="T"/>.</param>
        /// <param name="right">The <see cref="QualifiedMember{T}"/>.</param>
        /// <returns>True if not found equal.</returns>
        public static bool operator !=(T left, QualifiedMember<T> right) => !(left == right);

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/>. </summary>
        /// <param name="left">The <see cref="ISymbol"/>.</param>
        /// <param name="right">The <see cref="QualifiedMember{T}"/>.</param>
        /// <returns>True if found equal.</returns>
        public static bool operator ==(ISymbol left, QualifiedMember<T> right)
        {
            return left is T variable && variable == right;
        }

        /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/>. </summary>
        /// <param name="left">The <see cref="ISymbol"/>.</param>
        /// <param name="right">The <see cref="QualifiedMember{T}"/>.</param>
        /// <returns>True if not found equal.</returns>
        public static bool operator !=(ISymbol left, QualifiedMember<T> right)
        {
            return !(left == right);
        }

#pragma warning disable CA2225 // Operator overloads have named alternates
        /// <summary> Check if <paramref name="symbol"/> is the type described by this instance.</summary>
        /// <param name="symbol">The <typeparamref name="T"/>.</param>
        /// <returns>True if found equal.</returns>
        protected virtual bool Equals(T symbol)
#pragma warning restore CA2225 // Operator overloads have named alternates
        {
            if (symbol is null)
            {
                return false;
            }

            if (symbol.MetadataName != this.Name)
            {
                return false;
            }

            if (symbol.ContainingType == this.ContainingType)
            {
                return true;
            }

            if (symbol.IsStatic)
            {
                return false;
            }

            foreach (var @interface in symbol.ContainingType.AllInterfaces)
            {
                if (@interface == this.ContainingType)
                {
                    return true;
                }
            }

            return symbol.Name.IsParts(this.ContainingType.FullName, ".", this.Name);
        }
    }
}
