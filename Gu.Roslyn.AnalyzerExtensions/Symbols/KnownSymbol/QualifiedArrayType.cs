namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public class QualifiedArrayType : QualifiedType
    {
        private static readonly NamespaceParts NamespaceParts = NamespaceParts.Create("System.Array");

        public QualifiedArrayType(QualifiedType elementType)
            : base(elementType.FullName + "[]", NamespaceParts, elementType.Type + "[]")
        {
            this.ElementType = elementType;
        }

        public QualifiedType ElementType { get; }

        public override ITypeSymbol GetTypeSymbol(Compilation compilation) => compilation.CreateArrayTypeSymbol(this.ElementType.GetTypeSymbol(compilation));
    }
}
