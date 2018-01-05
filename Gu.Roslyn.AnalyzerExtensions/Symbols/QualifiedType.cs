namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    [System.Diagnostics.DebuggerDisplay("FullName: {FullName}")]
    public class QualifiedType
    {
        public static readonly QualifiedType Void = new QualifiedType("System.Void");
        public static readonly QualifiedType Object = new QualifiedType("System.Object");
        public static readonly QualifiedType Boolean = new QualifiedType("System.Boolean");

        public readonly string FullName;
        public readonly NamespaceParts Namespace;
        public readonly string Type;

        public QualifiedType(string fullName)
            : this(fullName, NamespaceParts.GetOrCreate(fullName), fullName.Substring(fullName.LastIndexOf('.') + 1))
        {
        }

        private QualifiedType(string fullName, NamespaceParts @namespace, string type)
        {
            this.FullName = fullName;
            this.Namespace = @namespace;
            this.Type = type;
        }

        public static bool operator ==(ITypeSymbol left, QualifiedType right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.MetadataName == right.Type &&
                   left.ContainingNamespace == right.Namespace;
        }

        public static bool operator !=(ITypeSymbol left, QualifiedType right) => !(left == right);
    }
}
