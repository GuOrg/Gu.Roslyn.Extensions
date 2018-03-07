#pragma warning disable 660,661 // using a hack with operator overloads
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    [DebuggerDisplay("{ContainingType.FullName,nq}.{Name,nq}")]
    public class QualifiedMember<T>
        where T : ISymbol
    {
        public readonly string Name;
        public readonly QualifiedType ContainingType;

        public QualifiedMember(QualifiedType containingType, string name)
        {
            this.Name = name;
            this.ContainingType = containingType;
        }

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
            return left is T && (T)left == right;
        }

        public static bool operator !=(ISymbol left, QualifiedMember<T> right)
        {
            return !(left == right);
        }
    }
}
