#pragma warning disable 660,661 // using a hack with operator overloads
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="ISymbol"/>.
    /// </summary>
    /// <typeparam name="T">The type of symbol</typeparam>
    [DebuggerDisplay("{ContainingType.FullName,nq}.{Name,nq}")]
    public abstract class QualifiedMember<T>
        where T : ISymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedMember{T}"/> class.
        /// </summary>
        /// <param name="containingType">The containing type</param>
        /// <param name="name">The name</param>
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

        public static bool operator ==(T left, QualifiedMember<T> right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left.MetadataName != right.Name)
            {
                return false;
            }

            if (left.ContainingType == right.ContainingType)
            {
                return true;
            }

            if (left.IsStatic)
            {
                return false;
            }

            foreach (var @interface in left.ContainingType.AllInterfaces)
            {
                if (@interface == right.ContainingType)
                {
                    return true;
                }
            }

            return left.Name.IsParts(right.ContainingType.FullName, ".", right.Name);
        }

        public static bool operator !=(T left, QualifiedMember<T> right) => !(left == right);

        public static bool operator ==(ISymbol left, QualifiedMember<T> right)
        {
            return left is T variable && variable == right;
        }

        public static bool operator !=(ISymbol left, QualifiedMember<T> right)
        {
            return !(left == right);
        }
    }
}
