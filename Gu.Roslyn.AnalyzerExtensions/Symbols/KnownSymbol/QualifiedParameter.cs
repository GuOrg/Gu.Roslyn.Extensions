namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with <see cref="Microsoft.CodeAnalysis.IParameterSymbol"/>
    /// </summary>
    public struct QualifiedParameter : IEquatable<QualifiedParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedParameter"/> struct.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        public QualifiedParameter(string name, QualifiedType type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public QualifiedType Type { get; }

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="IParameterSymbol"/></param>
        /// <param name="right">The <see cref="QualifiedParameter"/></param>
        /// <returns>True if found equal</returns>
        public static bool operator ==(IParameterSymbol left, QualifiedParameter right)
        {
            if (left == null)
            {
                return false;
            }

            if (right.Name is string name &&
                left.Name != name)
            {
                return false;
            }

            if (right.Type is QualifiedType type &&
                left.Type != type)
            {
                return false;
            }

            return true;
        }

        /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/> </summary>
        /// <param name="left">The <see cref="IParameterSymbol"/></param>
        /// <param name="right">The <see cref="QualifiedParameter"/></param>
        /// <returns>True if not found equal</returns>
        public static bool operator !=(IParameterSymbol left, QualifiedParameter right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Create a <see cref="QualifiedParameter"/> with name only.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="QualifiedParameter"/>.</returns>
        public static QualifiedParameter Create(string name) => new QualifiedParameter(name, null);

        /// <summary>
        /// Create a <see cref="QualifiedParameter"/> with name only.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="QualifiedParameter"/>.</returns>
        public static QualifiedParameter Create(QualifiedType type) => new QualifiedParameter(null, type);

        /// <inheritdoc />
        public bool Equals(QualifiedParameter other)
        {
            return string.Equals(this.Name, other.Name) && Equals(this.Type, other.Type);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is QualifiedParameter other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397) ^ (this.Type != null ? this.Type.GetHashCode() : 0);
            }
        }
    }
}
