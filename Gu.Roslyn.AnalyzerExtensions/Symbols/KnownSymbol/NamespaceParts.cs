namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// For comparison with roslyn <see cref="INamespaceSymbol"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{System.String.Join(\".\", Parts)}")]
    public sealed class NamespaceParts
    {
        private readonly ImmutableList<string> parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceParts"/> class.
        /// </summary>
        /// <param name="parts">The strings that make up the namespace split at the dots.</param>
        private NamespaceParts(ImmutableList<string> parts)
        {
            this.parts = parts;
        }

        /// <summary> Check if <paramref name="left"/> is the namespace described by <paramref name="right"/>. </summary>
        /// <param name="left">The <see cref="INamespaceSymbol"/>.</param>
        /// <param name="right">The <see cref="NamespaceParts"/>.</param>
        /// <returns>True if found equal.</returns>
        public static bool operator ==(INamespaceSymbol left, NamespaceParts right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            var ns = left;
            for (var i = right.parts.Count - 1; i >= 0; i--)
            {
                if (ns is null || ns.IsGlobalNamespace)
                {
                    return false;
                }

                if (ns.Name != right.parts[i])
                {
                    return false;
                }

                ns = ns.ContainingNamespace;
            }

            return ns?.IsGlobalNamespace == true;
        }

        /// <summary> Check if <paramref name="left"/> is not the namespace described by <paramref name="right"/>. </summary>
        /// <param name="left">The <see cref="INamespaceSymbol"/>.</param>
        /// <param name="right">The <see cref="NamespaceParts"/>.</param>
        /// <returns>True if found not equal.</returns>
        public static bool operator !=(INamespaceSymbol left, NamespaceParts right) => !(left == right);

        /// <summary>
        /// Create a new instance from <paramref name="fullName"/>.
        /// </summary>
        /// <param name="fullName">The namespace name ex: 'System.Collections'.</param>
        /// <returns>The created instance.</returns>
        public static NamespaceParts Create(string fullName)
        {
            if (fullName is null)
            {
                throw new System.ArgumentNullException(nameof(fullName));
            }

            if (fullName.IndexOf('`') is var i &&
                i > 0)
            {
                fullName = fullName.Substring(0, i);
            }

            var parts = fullName.Split('.').ToImmutableList();
            System.Diagnostics.Debug.Assert(parts.Count != 0, "Parts.Length != 0");
            return new NamespaceParts(parts.RemoveAt(parts.Count - 1));
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is NamespaceParts namespaceParts &&
                   this.Equals(namespaceParts);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.parts[this.parts.Count - 1].GetHashCode();
        }

        /// <summary> Check if this instance describes the namespace <paramref name="nameSyntax"/>. </summary>
        /// <param name="nameSyntax">The <see cref="NameSyntax"/>.</param>
        /// <returns>True if found to be the same namespace.</returns>
        internal bool Matches(NameSyntax nameSyntax)
        {
            return this.Matches(nameSyntax, this.parts.Count - 1);
        }

        private bool Matches(NameSyntax nameSyntax, int index)
        {
            if (nameSyntax is IdentifierNameSyntax identifier)
            {
                return index == 0 &&
                       identifier.Identifier.ValueText == this.parts[0];
            }

            if (nameSyntax is QualifiedNameSyntax qns)
            {
                if (index < 1)
                {
                    return false;
                }

                if (qns.Right.Identifier.ValueText != this.parts[index])
                {
                    return false;
                }

                return this.Matches(qns.Left, index - 1);
            }

            return false;
        }

        private bool Equals(NamespaceParts other)
        {
            if (this.parts.Count == other.parts.Count)
            {
                for (var i = 0; i < this.parts.Count; i++)
                {
                    if (this.parts[i] != other.parts[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
