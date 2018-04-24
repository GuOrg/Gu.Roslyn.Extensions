namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// For comparison with roslyn types.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("{this.FullName}")]
    public class QualifiedType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedType"/> class.
        /// </summary>
        /// <param name="fullName">For example 'System.String'</param>
        /// <param name="alias">For example 'string'</param>
        public QualifiedType(string fullName, string alias = null)
            : this(fullName, NamespaceParts.Create(fullName), fullName.Substring(fullName.LastIndexOf('.') + 1), alias)
        {
        }

        private QualifiedType(string fullName, NamespaceParts @namespace, string type, string alias = null)
        {
            this.FullName = fullName;
            this.Namespace = @namespace;
            this.Type = type;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the namespace
        /// </summary>
        public NamespaceParts Namespace { get; }

        /// <summary>
        /// Gets the type name
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the type alias, can be null.
        /// </summary>
        public string Alias { get; }

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

            return NameEquals(left.MetadataName, right) &&
                   left.ContainingNamespace == right.Namespace;
        }

        public static bool operator !=(ITypeSymbol left, QualifiedType right) => !(left == right);

        public static bool operator ==(BaseTypeSyntax left, QualifiedType right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.Type == right;
        }

        public static bool operator !=(BaseTypeSyntax left, QualifiedType right) => !(left == right);

        public static bool operator ==(TypeSyntax left, QualifiedType right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left is SimpleNameSyntax simple)
            {
                return NameEquals(simple.Identifier.ValueText, right);
            }

            if (left is QualifiedNameSyntax qualified)
            {
                return NameEquals(qualified.Right.Identifier.ValueText, right) &&
                       right.Namespace.Matches(qualified.Left);
            }

            return false;
        }

        public static bool operator !=(TypeSyntax left, QualifiedType right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((QualifiedType)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.FullName.GetHashCode();
        }

        private static bool NameEquals(string left, QualifiedType right)
        {
            return left == right.Type ||
                   (right.Alias != null && left == right.Alias);
        }

        protected bool Equals(QualifiedType other)
        {
            return string.Equals(this.FullName, other.FullName);
        }
    }
}
