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

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="ITypeSymbol"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if found equal</returns>
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

        /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="ITypeSymbol"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if not found equal</returns>
        public static bool operator !=(ITypeSymbol left, QualifiedType right) => !(left == right);

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="BaseTypeSyntax"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if found equal</returns>
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

        /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="BaseTypeSyntax"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if not found equal</returns>
        public static bool operator !=(BaseTypeSyntax left, QualifiedType right) => !(left == right);

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="TypeSyntax"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if found equal</returns>
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

        /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="TypeSyntax"/></param>
        /// <param name="right">The <see cref="QualifiedType"/></param>
        /// <returns>True if not found equal</returns>
        public static bool operator !=(TypeSyntax left, QualifiedType right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
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

        /// <summary>
        /// Check if equal.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>True if equal.</returns>
        protected bool Equals(QualifiedType other)
        {
            return string.Equals(this.FullName, other.FullName);
        }

        private static bool NameEquals(string left, QualifiedType right)
        {
            return left == right.Type ||
                   (right.Alias != null &&
                    left == right.Alias);
        }

        /// <summary>
        /// Contains types from the System namespace.
        /// </summary>
        public static class System
        {
            /// <summary> System.Void </summary>
            public static readonly QualifiedType Void = new QualifiedType("System.Void", "void");

            /// <summary> System.Object </summary>
            public static readonly QualifiedType Object = new QualifiedType("System.Object", "object");

            /// <summary> System.Nullable`1 </summary>
            public static readonly QualifiedType NullableOfT = new QualifiedType("System.Nullable`1");

            /// <summary> System.Boolean </summary>
            public static readonly QualifiedType Boolean = new QualifiedType("System.Boolean", "bool");

            /// <summary> System.String </summary>
            public static readonly QualifiedType String = new QualifiedType("System.String", "string");

            /// <summary> System.Linq </summary>
            public static class Linq
            {
                /// <summary> System.Linq.Expressions.Expression </summary>
                internal static readonly QualifiedType Expression = new QualifiedType("System.Linq.Expressions.Expression");
            }

            /// <summary> System.Runtime </summary>
            public static class Runtime
            {
                /// <summary> System.Runtime.CompilerServices </summary>
                public static class CompilerServices
                {
                    /// <summary> System.Runtime.CompilerServices.CallerMemberNameAttribute </summary>
                    public static readonly QualifiedType CallerMemberNameAttribute = new QualifiedType("System.Runtime.CompilerServices.CallerMemberNameAttribute");
                }
            }
        }
    }
}
