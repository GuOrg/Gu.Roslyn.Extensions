namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with <see cref="ITypeSymbol"/>.
    /// </summary>
    public class QualifiedArrayType : QualifiedType
    {
        private static readonly NamespaceParts NamespaceParts = NamespaceParts.Create("System.Array");

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedArrayType"/> class.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        public QualifiedArrayType(QualifiedType elementType)
            : base(elementType.FullName + "[]", NamespaceParts, elementType.Type + "[]")
        {
            this.ElementType = elementType;
        }

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public QualifiedType ElementType { get; }

        /// <inheritdoc />
        public override ITypeSymbol GetTypeSymbol(Compilation compilation) => compilation.CreateArrayTypeSymbol(this.ElementType.GetTypeSymbol(compilation));
    }
}
